﻿namespace MvcProject.Configuration;

public class MailSettings
{
    public string? SmtpServer { get; set; }
    public int SmtpPort { get; set; }
    public string? SmtpUsername { get; set; }
    public string? SmtpPassword { get; set; }
    public string? SenderName { get; set; }
    public string? SenderEmail { get; set; }
    public bool UseSsl { get; set; }
}
