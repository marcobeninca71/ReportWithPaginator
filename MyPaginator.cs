using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace WpfAppReports
{
    public class MyPaginator : DocumentPaginator
    {
        private Size pageSize = new Size(100, 300);

        public List<Model> rows { get; set; }

        public override bool IsPageCountValid { get { return counter >= rows.Count; } }

        public override int PageCount { get { return rows.Count; } }

        public override Size PageSize { get { return pageSize; } set { pageSize = value; } }

        public override IDocumentPaginatorSource Source { get { return null; } }

        private int counter = 0;
        public override DocumentPage GetPage(int pageNumber)
        {
            Model data = counter < rows.Count ? rows[counter] : null;
            var page = new PrintControl(data);
            page.Measure(PageSize);
            page.Arrange(new Rect(new Point(0, 0), PageSize));
            counter++;
            return new DocumentPage(page);
        }
    }
}
