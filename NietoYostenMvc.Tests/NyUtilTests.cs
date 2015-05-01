using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using NietoYostenMvc.Code;
using Xunit;
using Xunit.Abstractions;

namespace NietoYostenMvc.Tests
{
    public class NyUtilTests
    {
        private readonly ITestOutputHelper output;

        public NyUtilTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test_Convert_List_To_Table()
        {
            List<dynamic> list = new List<dynamic>();

            for (int i = 0; i < 13; i++)
            {
                dynamic item = new ExpandoObject();
                item.ID = i;
                item.FileName = string.Format("file{0:D3}", i);

                list.Add(item);
            }

            IEnumerable<IEnumerable<dynamic>> table = NyUtil.ConvertListToTable(list, 4);

            // Check row number
            Assert.Equal(4, table.Count());

            // Check column number
            IEnumerable<dynamic> firstRow = table.First();
            IEnumerable<dynamic> lastRow = table.Last();
            Assert.Equal(4, firstRow.Count());
            
            // Check table contents
            dynamic firstItem = firstRow.First();
            Assert.Equal(firstItem.FileName, "file000");
            Assert.Equal(lastRow.First().FileName, "file012");
        }

        [Fact]
        public void Test_Convert_EmptyList_To_Table()
        {
            List<dynamic> list = new List<dynamic>();
            IEnumerable<IEnumerable<dynamic>> table = NyUtil.ConvertListToTable(list, 4);

            Assert.Empty(table);
            Assert.Equal(0, table.Count());
        }
    }
}
