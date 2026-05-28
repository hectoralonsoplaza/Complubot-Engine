using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System.IO;

public class RecorderManager : MonoBehaviour
{
    [Header("Configuración del Vídeo")]
    public int videoWidth = 1024;   
    public int videoHeight = 576;
    public int frameRate = 24;

    private Process ffmpegProcess;
    private BinaryWriter ffmpegWriter;
    private bool isRecording = false;
    private Texture2D screenShot;
    private RenderTexture rt;

    public void StartRecording()
    {
        if (isRecording) return;

        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string fileName = "CircuitVideo_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".mp4";
        string outputPath = Path.Combine(desktopPath, fileName);

        string ffmpegPath = Path.Combine(Application.streamingAssetsPath, "ffmpeg.exe");

        if (!File.Exists(ffmpegPath))
        {
            UnityEngine.Debug.LogError("No se encontró ffmpeg.exe en la carpeta Assets/StreamingAssets/");
            return;
        }

        
        string arguments = $"-f rawvideo -pix_fmt rgba -s {videoWidth}x{videoHeight} -r {frameRate} -i - " +
                           $"-c:v libx264 -pix_fmt yuv420p -preset ultrafast -tune zerolatency -y \"{outputPath}\"";

        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardInput = true
        };

        ffmpegProcess = new Process { StartInfo = startInfo };
        ffmpegProcess.Start();
        ffmpegWriter = new BinaryWriter(ffmpegProcess.StandardInput.BaseStream);

        
        rt = new RenderTexture(videoWidth, videoHeight, 0, RenderTextureFormat.ARGB32);
        rt.Create();
        screenShot = new Texture2D(videoWidth, videoHeight, TextureFormat.RGBA32, false);

        isRecording = true;
        StartCoroutine(RecordFrameCoroutine());
        UnityEngine.Debug.Log("Grabación nativa de pantalla iniciada.");
    }

    public void StopRecording()
    {
        if (!isRecording) return;

        isRecording = false;
        StopAllCoroutines();

        
        if (rt != null) { rt.Release(); Destroy(rt); }
        if (screenShot != null) Destroy(screenShot);

        
        if (ffmpegWriter != null)
        {
            ffmpegWriter.Flush();
            ffmpegWriter.Close();
        }

        if (ffmpegProcess != null && !ffmpegProcess.HasExited)
        {
            ffmpegProcess.WaitForExit();
            ffmpegProcess.Dispose();
        }

        UnityEngine.Debug.Log("Grabación finalizada con éxito.");
    }

    private IEnumerator RecordFrameCoroutine()
    {
        float timeBetweenFrames = 1f / frameRate;

        while (isRecording)
        {
            
            yield return new WaitForEndOfFrame();

            
            ScreenCapture.CaptureScreenshotIntoRenderTexture(rt);

            
            RenderTexture oldActive = RenderTexture.active;
            RenderTexture.active = rt;

            screenShot.ReadPixels(new Rect(0, 0, videoWidth, videoHeight), 0, 0);
            screenShot.Apply();

            RenderTexture.active = oldActive;

            
            byte[] rawBytes = screenShot.GetRawTextureData();

            
            try
            {
                ffmpegWriter.Write(rawBytes);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError("Error FFmpeg: " + e.Message);
                StopRecording();
            }

            yield return new WaitForSecondsRealtime(timeBetweenFrames);
        }
    }

    private void OnApplicationQuit()
    {
        if (isRecording) StopRecording();
    }
}