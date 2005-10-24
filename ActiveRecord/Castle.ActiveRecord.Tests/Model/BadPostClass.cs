using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.ActiveRecord.Tests.Model
{
    /// <summary>
    /// Used to verify that you can't map an AR class as a [Property]
    /// </summary>
    [ActiveRecord]
    public class BadPostClass
    {
        [Property]
        public Blog Blog
        {
            get { return null; }
            set { }
        }
    }
}
