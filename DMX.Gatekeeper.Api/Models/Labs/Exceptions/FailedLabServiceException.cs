﻿// ---------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// ---------------------------------------------------------------

using System;
using Xeptions;

namespace DMX.Gatekeeper.Api.Models.Labs.Exceptions
{
    public class FailedLabServiceException : Xeption
    {
        public FailedLabServiceException(Exception innerException)
            : base(message: "Failed lab service error occured, contact support.",
                  innerException)
        { }
    }
}
