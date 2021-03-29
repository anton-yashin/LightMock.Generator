using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightMock.Generator.Tests.ExpressionReflect.Model
{
    static class CustomerExtensions
    {
		public static int GetDefaultAgeEx(this Customer customer, int value)
		{
			return Customer.AgeConstant + value;
		}

		public static int GetDefaultAgeEx(this Customer customer)
		{
			return customer.Age + Customer.AgeConstant;
		}
	}
}
