﻿using Mst.Logging.Enums;

namespace Mst.Logging.CustomLogs;

public interface ILog
{
    LogLevelEnum Level { get; set; }



    string? Namespace { get; set; }

    string? ClassName { get; set; }

    string? MethodName { get; set; }

    string? MethodNameReceive { get; set; }



    string? RemoteIP { get; set; }

    string? Username { get; set; }

    string? RequestPath { get; set; }

    string? HttpReferrer { get; set; }



    string? Message { get; set; }

    string? Parameters { get; set; }

    ExceptionsCustom Exceptions { get; set; }
}
