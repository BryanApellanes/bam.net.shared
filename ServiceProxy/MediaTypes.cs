﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Bam.Net.ServiceProxy
{
    public class MediaTypes
    {
        public const string Json = "application/json; charset=utf-8";
        public const string BamPipeline = "application/vnd.bam.pipeline";
        public const string AsymmetricCipher = "application/vnd.bam.pipeline+json+rsa+gzip";
        public const string SymmetricCipher = "application/vnd.bam.pipeline+json+aes+gzip";
    }
}
