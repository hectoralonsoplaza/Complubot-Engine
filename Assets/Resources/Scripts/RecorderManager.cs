using UnityEngine;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEditor.Recorder.Encoder;
using System.IO;

public class RecorderManager : MonoBehaviour
{
    RecorderControllerSettings controllerSettings;
    RecorderController recorderController;

    bool isRecording;

    public void StartRecording()
    {
        if (isRecording) return;

        controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        recorderController = new RecorderController(controllerSettings);

        var movieRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();

        movieRecorder.name = "CircuitRecorder";
        movieRecorder.Enabled = true;

        var encoder = new CoreEncoderSettings
        {
            EncodingQuality = CoreEncoderSettings.VideoEncodingQuality.High
        };

        movieRecorder.EncoderSettings = encoder;

        movieRecorder.ImageInputSettings = new GameViewInputSettings
        {
            OutputWidth = 1920,
            OutputHeight = 1080
        };

        movieRecorder.AudioInputSettings.PreserveAudio = true;

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string fileName = "CircuitVideo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss");

        movieRecorder.OutputFile = Path.Combine(desktopPath, fileName);

        controllerSettings.AddRecorderSettings(movieRecorder);
        controllerSettings.SetRecordModeToManual();

        recorderController.PrepareRecording();
        recorderController.StartRecording();

        isRecording = true;

        Debug.Log(" Grabacion iniciada");
    }

    public void StopRecording()
    {
        if (!isRecording) return;

        recorderController.StopRecording();
        isRecording = false;

        Debug.Log(" Grabacion finalizada");
    }
}