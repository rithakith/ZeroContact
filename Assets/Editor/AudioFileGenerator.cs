using UnityEngine;
using UnityEditor;
using System.IO;

public class AudioFileGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Test Audio Files")]
    public static void ShowWindow()
    {
        GetWindow<AudioFileGenerator>("Audio File Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Test Audio File Generator", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("This will create basic test audio files for your main menu:", EditorStyles.label);
        GUILayout.Label("• button_click.wav - Button click sound", EditorStyles.label);
        GUILayout.Label("• button_hover.wav - Button hover sound", EditorStyles.label);
        GUILayout.Label("• background_music.wav - Background music", EditorStyles.label);
        GUILayout.Label("• ambient_wind.wav - Ambient wind sound", EditorStyles.label);

        GUILayout.Space(10);

        if (GUILayout.Button("Generate Test Audio Files"))
        {
            GenerateTestAudioFiles();
        }
    }

    private void GenerateTestAudioFiles()
    {
        try
        {
            // Ensure Audio folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Audio"))
            {
                AssetDatabase.CreateFolder("Assets", "Audio");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            // Generate button click sound (short beep)
            CreateAudioClip("button_click", 0.1f, 800f, 0.5f, "Assets/Audio/button_click.wav");
            
            // Generate button hover sound (soft beep)
            CreateAudioClip("button_hover", 0.05f, 600f, 0.3f, "Assets/Audio/button_hover.wav");
            
            // Generate background music (simple melody)
            CreateBackgroundMusic("background_music", "Assets/Audio/background_music.wav");
            
            // Generate ambient wind sound
            CreateAmbientSound("ambient_wind", "Assets/Audio/ambient_wind.wav");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("=== AUDIO DEBUG: Test audio files generated successfully! ===");
            Debug.Log("=== AUDIO DEBUG: Files created in Assets/Audio/ ===");
            EditorUtility.DisplayDialog("Success", "Test audio files generated successfully!\n\nLocation: Assets/Audio/\n\nNow regenerate your MainMenu scene to use these sounds.", "OK");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error generating audio files: {e.Message}");
            EditorUtility.DisplayDialog("Error", $"Error generating audio files:\n{e.Message}", "OK");
        }
    }

    private void CreateAudioClip(string name, float duration, float frequency, float volume, string path)
    {
        int sampleRate = 44100;
        int samples = Mathf.RoundToInt(duration * sampleRate);
        
        float[] data = new float[samples];
        
        for (int i = 0; i < samples; i++)
        {
            float time = (float)i / sampleRate;
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * time);
            
            // Add fade in/out to avoid clicks
            float fadeIn = Mathf.Clamp01(time / 0.01f);
            float fadeOut = Mathf.Clamp01((duration - time) / 0.01f);
            float envelope = fadeIn * fadeOut;
            
            data[i] = wave * volume * envelope;
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        
        // Save as WAV file
        byte[] wavData = ConvertToWAV(data, sampleRate);
        File.WriteAllBytes(path, wavData);
        
        Debug.Log($"=== AUDIO DEBUG: Created {name} at {path} ===");
    }

    private void CreateBackgroundMusic(string name, string path)
    {
        int sampleRate = 44100;
        float duration = 5f; // 5 second loop
        int samples = Mathf.RoundToInt(duration * sampleRate);
        
        float[] data = new float[samples];
        
        // Create a simple melody with multiple frequencies
        float[] frequencies = { 440f, 523.25f, 659.25f, 783.99f }; // A, C, E, G
        float noteDuration = 0.5f;
        int notesPerSecond = Mathf.RoundToInt(1f / noteDuration);
        
        for (int i = 0; i < samples; i++)
        {
            float time = (float)i / sampleRate;
            float noteTime = time % noteDuration;
            int noteIndex = Mathf.FloorToInt(time / noteDuration) % frequencies.Length;
            
            float frequency = frequencies[noteIndex];
            float wave = Mathf.Sin(2f * Mathf.PI * frequency * noteTime);
            
            // Add some harmonics for richer sound
            wave += 0.3f * Mathf.Sin(2f * Mathf.PI * frequency * 2f * noteTime);
            wave += 0.1f * Mathf.Sin(2f * Mathf.PI * frequency * 3f * noteTime);
            
            data[i] = wave * 0.2f; // Lower volume for background music
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        
        // Save as WAV file
        byte[] wavData = ConvertToWAV(data, sampleRate);
        File.WriteAllBytes(path, wavData);
        
        Debug.Log($"=== AUDIO DEBUG: Created {name} at {path} ===");
    }

    private void CreateAmbientSound(string name, string path)
    {
        int sampleRate = 44100;
        float duration = 3f; // 3 second loop
        int samples = Mathf.RoundToInt(duration * sampleRate);
        
        float[] data = new float[samples];
        
        // Create wind-like ambient sound with noise and low frequencies
        for (int i = 0; i < samples; i++)
        {
            float time = (float)i / sampleRate;
            
            // Base wind sound (low frequency noise)
            float wind = Mathf.PerlinNoise(time * 2f, 0f) * 0.5f;
            
            // Add some variation
            wind += Mathf.PerlinNoise(time * 5f, 0f) * 0.3f;
            wind += Mathf.PerlinNoise(time * 10f, 0f) * 0.1f;
            
            // Add some low frequency oscillation
            wind += Mathf.Sin(2f * Mathf.PI * 0.5f * time) * 0.1f;
            
            data[i] = wind * 0.15f; // Low volume for ambient
        }

        AudioClip clip = AudioClip.Create(name, samples, 1, sampleRate, false);
        clip.SetData(data, 0);
        
        // Save as WAV file
        byte[] wavData = ConvertToWAV(data, sampleRate);
        File.WriteAllBytes(path, wavData);
        
        Debug.Log($"=== AUDIO DEBUG: Created {name} at {path} ===");
    }

    private byte[] ConvertToWAV(float[] audioData, int sampleRate)
    {
        // Simple WAV file format
        int dataSize = audioData.Length * 2; // 16-bit samples
        int fileSize = 36 + dataSize;
        
        using (MemoryStream stream = new MemoryStream())
        using (BinaryWriter writer = new BinaryWriter(stream))
        {
            // WAV header
            writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            writer.Write(fileSize);
            writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
            
            // Format chunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
            writer.Write(16); // Chunk size
            writer.Write((short)1); // Audio format (PCM)
            writer.Write((short)1); // Number of channels
            writer.Write(sampleRate); // Sample rate
            writer.Write(sampleRate * 2); // Byte rate
            writer.Write((short)2); // Block align
            writer.Write((short)16); // Bits per sample
            
            // Data chunk
            writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            writer.Write(dataSize);
            
            // Convert float data to 16-bit PCM
            for (int i = 0; i < audioData.Length; i++)
            {
                short sample = (short)(audioData[i] * 32767f);
                writer.Write(sample);
            }
            
            return stream.ToArray();
        }
    }
} 