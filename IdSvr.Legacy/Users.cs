﻿using System.Collections.Generic;
using IdentityServer3.Core.Services.InMemory;

namespace IdSvr.Legacy
{
    public class Users
    {
        public static List<InMemoryUser> Get() => new List<InMemoryUser>
        {
            new InMemoryUser
            {
                Username = "bob",
                Password = "secret",
                Subject = "1"
            },
            new InMemoryUser
            {
                Username = "alice",
                Password = "secret",
                Subject = "2"
            }
        };
    }
}