﻿namespace LoggingWithStreamWriter;

public class LogWriter : IDisposable
{
    private readonly StreamWriter _writer;

    public LogWriter(string path)
    {
        _writer = new StreamWriter(path, append: true)
        {
            AutoFlush = true
        };
    }

    public void WriteLog(string level, string message)
    {
        var tiemstamp = DateTime.Now.ToString("s"); // ISO 8601 format
        _writer.WriteLine($"{tiemstamp} [{level}] {message}");
    }

    public void Dispose()
    {
        _writer?.Dispose();
    }
}