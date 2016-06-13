using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProjectCorrection.Database;
using ProjectCorrection.Main;
using Microsoft.Kinect;
using System.IO;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.ComponentModel;
//using Microsoft.Speech.Synthesis;
using ScriptCs.SpeakR;
using ScriptCs.Contracts;
using System.Speech;
using System.Speech.Synthesis;

namespace ProjectCorrection.Enrollment
{
    /// <summary>
    /// 자세등록화면을 나타내는 클래스입니다.
    /// </summary>
    public partial class PostureEnrollmentScreen : Page
    {
        private PostureEnrollmentProcess eProcess;  //자세등록을 처리하기 위한 참조
        private string id;                          //현재 유저 id
        /// <summary>
        /// Active Kinect sensor.
        /// </summary>
        private KinectSensor sensor;
        private SpeechSynthesizer speechSynthesizer;

        /// <summary>
        /// Speech recognition engine using audio data from Kinect.
        /// </summary>
        private SpeechRecognitionEngine speechEngine;
        /// <summary>
        /// 이 클래스의 생성자 입니다. 생성되는 순간 자세등록 프로세스가 진행됩니다.
        /// </summary>
        /// <param name="id">현재 유저 id</param>
        public PostureEnrollmentScreen(string id)
        {
            InitializeComponent();
            this.id = id;


       //     speechSynthesizer = new SpeechSynthesizer();
        //    speechSynthesizer.SetOutputToDefaultAudioDevice();
        //    speechSynthesizer.Volume = 100;

            eProcess = new PostureEnrollmentProcess(this.id, image1, textBox1);
            eProcess.Process();


            foreach (var potentialSensor in KinectSensor.KinectSensors)
            {
                if (potentialSensor.Status == KinectStatus.Connected)
                {
                    this.sensor = potentialSensor;
                    break;
                }
            }

            if (null != this.sensor)
            {
                try
                {

                    // Start the sensor!
                    this.sensor.Start();
                }
                catch (IOException)
                {
                    // Some other application is streaming from the same Kinect sensor
                    this.sensor = null;
                }
            }

            if (null == this.sensor)
            {
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();

            if (null != ri)
            {


                this.speechEngine = new SpeechRecognitionEngine(ri.Id);

                /****************************************************************
                * 
                * Use this code to create grammar programmatically rather than from
                * a grammar file.
                * 
                * var directions = new Choices();
                * directions.Add(new SemanticResultValue("forward", "FORWARD"));
                * directions.Add(new SemanticResultValue("forwards", "FORWARD"));
                * directions.Add(new SemanticResultValue("straight", "FORWARD"));
                * directions.Add(new SemanticResultValue("backward", "BACKWARD"));
                * directions.Add(new SemanticResultValue("backwards", "BACKWARD"));
                * directions.Add(new SemanticResultValue("back", "BACKWARD"));
                * directions.Add(new SemanticResultValue("turn left", "LEFT"));
                * directions.Add(new SemanticResultValue("turn right", "RIGHT"));
                *
                * var gb = new GrammarBuilder { Culture = ri.Culture };
                * gb.Append(directions);
                *
                * var g = new Grammar(gb);
                * 
                ****************************************************************/

                // Create a grammar from grammar definition XML file.
                using (var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(Properties.Resources.SpeechGrammar)))
                {
                    var g = new Grammar(memoryStream);
                    speechEngine.LoadGrammar(g);
                }

                speechEngine.SpeechRecognized += SpeechRecognized;
                speechEngine.SpeechRecognitionRejected += SpeechRejected;

                speechEngine.SetInputToAudioStream(
                    sensor.AudioSource.Start(), new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
                speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            }

        }


        private static RecognizerInfo GetKinectRecognizer()
        {
            foreach (RecognizerInfo recognizer in SpeechRecognitionEngine.InstalledRecognizers())
            {
                string value;
                recognizer.AdditionalInfo.TryGetValue("Kinect", out value);
                if ("True".Equals(value, StringComparison.OrdinalIgnoreCase) && "en-US".Equals(recognizer.Culture.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return recognizer;
                }
            }

            return null;
        }

        /// <summary>
        /// Execute uninitialization tasks.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">event arguments.</param>
        private void WindowClosing(object sender, CancelEventArgs e)
        {
            if (null != this.sensor)
            {
                this.sensor.AudioSource.Stop();

                this.sensor.Stop();
                this.sensor = null;
            }

            if (null != this.speechEngine)
            {
                this.speechEngine.SpeechRecognized -= SpeechRecognized;
                this.speechEngine.SpeechRecognitionRejected -= SpeechRejected;
                this.speechEngine.RecognizeAsyncStop();
            }
        }

        private void SpeechRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            
        }



        /// <summary>
        /// 인식된 스피치 이벤트를 처리하는 핸들러 입니다.
        /// </summary>
        /// <param name="sender">이벤트를 보내는 오브젝트</param>
        /// <param name="e">스피치 인식 이벤트</param>
        private void SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            const double ConfidenceThreshold = 0.3; //소리를 인식하기 위한 소리 크기 기준

            if (e.Result.Confidence >= ConfidenceThreshold)
            {
                switch (e.Result.Semantics.Value.ToString())
                {
                    case "CAPTURE":
                            eProcess.CaptureMyPosture();
                    //        speechSynthesizer.Speak("Complete Capture");
                            MessageBox.Show("등록이 완료되었습니다.", "자세등록"); 
                        break;
                    case "FINISH":
                        if (this.NavigationService != null)
                        {
                //            speechSynthesizer.Speak("Back to MainScreen");
                            MessageBox.Show("메인화면으로 돌아갑니다.", "메인화면");
                            this.NavigationService.Navigate(new MainScreen(id));                         
                        }
                        break;
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)   //해당 버튼을 클릭하면 자세를 등록하는 버튼 이벤트 처리기입니다.
        {  
            eProcess.CaptureMyPosture();
    //        speechSynthesizer.Speak("Complete Capture");
            MessageBox.Show("등록이 완료되었습니다.", "자세등록"); 
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)   //해당 버튼을 클릭하면 메인화면으로 돌아가는 이벤트 처리기입니다.
        {
    //        speechSynthesizer.Speak("Back to MainScreen");
            MessageBox.Show("메인화면으로 돌아갑니다.", "메인화면");
            this.NavigationService.Navigate(new MainScreen(id));
        }
    }
}