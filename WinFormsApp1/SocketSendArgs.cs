﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public record SocketSendArgs(byte[] Bytes, IPEndPoint Dist);
}
