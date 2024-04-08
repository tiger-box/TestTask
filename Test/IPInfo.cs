using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
	internal class IPInfo
	{
		public IPAddress? ipAddress = null;
		public int count { get; private set; } = 1;

		public void IncrementCount()
		{
			count++;
		}

	}
}
