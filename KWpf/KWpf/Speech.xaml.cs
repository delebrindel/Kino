using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;

namespace KWpf
{
    /// <summary>
    /// Interaction logic for Speech.xaml
    /// </summary>
    public partial class Speech : Window
    {
        public KinectSensor CurrentSensor;
        public SpeechRecognitionEngine speechRecognizer;
        
        //Get Recognizer Info
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }
        public Speech()
        {
            InitializeComponent();
            InitializeKinect();
        }


        //Initilaize the kinect
        private KinectSensor InitializeKinect()
        {
            //get the first available sensor and set it to the current sensor variable
            CurrentSensor = KinectSensor.KinectSensors
                                  .FirstOrDefault(s => s.Status == KinectStatus.Connected);
            speechRecognizer = CreateSpeechRecognizer();
            //Start the sensor
            CurrentSensor.Start();
            //then run the start method to start streaming audio
            Start();
            return CurrentSensor;
        }

        //Start streaming audio
        private void Start()
        {
            //set sensor audio source to variable
            var audioSource = CurrentSensor.AudioSource;
            //Set the beam angle mode - the direction the audio beam is pointing
            //we want it to be set to adaptive
            audioSource.BeamAngleMode = BeamAngleMode.Adaptive;
            //start the audiosource 
            var kinectStream = audioSource.Start();
            //configure incoming audio stream
            speechRecognizer.SetInputToAudioStream(
                kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            //make sure the recognizer does not stop after completing     
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            //reduce background and ambient noise for better accuracy
            CurrentSensor.AudioSource.EchoCancellationMode = EchoCancellationMode.None;
            CurrentSensor.AudioSource.AutomaticGainControlEnabled = false;
        }

        //here is the fun part: create the speech recognizer
        private SpeechRecognitionEngine CreateSpeechRecognizer()
        {
            //set recognizer info
            RecognizerInfo ri = GetKinectRecognizer();
            //create instance of SRE
            SpeechRecognitionEngine sre;
            sre = new SpeechRecognitionEngine(ri.Id);

            //Now we need to add the words we want our program to recognise
            var grammar = new Choices();
            grammar.Add("hello");
            grammar.Add("goodbye");

            //set culture - language, country/region
            var gb = new GrammarBuilder { Culture = ri.Culture };
            gb.Append(grammar);

            //set up the grammar builder
            var g = new Grammar(gb);
            sre.LoadGrammar(g);

            //Set events for recognizing, hypothesising and rejecting speech
            sre.SpeechRecognized += SreSpeechRecognized;
            sre.SpeechHypothesized += SreSpeechHypothesized;
            sre.SpeechRecognitionRejected += SreSpeechRecognitionRejected;
            return sre;
        }

        //if speech is rejected
        private void RejectSpeech(RecognitionResult result)
        {
            textBox2.Text = "Pardon Moi?";
        }

        private void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            RejectSpeech(e.Result);
        }

        //hypothesized result
        private void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            textBox1.Text = "Hypothesized: " + e.Result.Text + " " + e.Result.Confidence;
        }

        //Speech is recognised
        private void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //Very important! - change this value to adjust accuracy - the higher the value
            //the more accurate it will have to be, lower it if it is not recognizing you
            if (e.Result.Confidence < .4)
            {
                RejectSpeech(e.Result);
            }
            //and finally, here we set what we want to happen when 
            //the SRE recognizes a word
            switch (e.Result.Text.ToUpperInvariant())
            {
                case "HELLO":
                    textBox3.Text = "Hi there.";
                    break;
                case "GOODBYE":
                    textBox3.Text = "Goodbye then.";
                    break;
                default:
                    break;
            }
        }


    }
}
