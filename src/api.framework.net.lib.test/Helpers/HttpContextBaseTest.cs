using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace api.framework.net.Lib.Test.Helpers
{
    public class HttpContextBaseTest : HttpContextBase
    {
        public string _File = string.Empty;
        public string _Data = string.Empty;
        public NameValueCollection _FormData = new NameValueCollection();
        public HttpContextBaseTest() : base()
        {

        }
        public HttpContextBaseTest(string data) : base()
        {
            this._Data = data;
        }

        public HttpContextBaseTest(string file, NameValueCollection formData) : base()
        {
            this._File = file;
            this._FormData = formData;
        }

        public override HttpRequestBase Request
        {
            get
            {
                if (!string.IsNullOrEmpty(_File))
                {
                    return new HttpRequestBaseTest(this._File, this._FormData);
                }
                else if (!string.IsNullOrEmpty(_Data))
                {
                    return new HttpRequestBaseTest(this._Data);
                }
                else
                {
                    return new HttpRequestBaseTest();
                }
            }
        }
    }

    public class HttpRequestBaseTest : HttpRequestBase
    {
        public string _File = string.Empty;
        public string _Data = string.Empty;
        public NameValueCollection FormData = new NameValueCollection();
        public HttpRequestBaseTest() : base()
        { }

        public HttpRequestBaseTest(string data) : base()
        {
            this._Data = data;
        }
        public HttpRequestBaseTest(string _file, NameValueCollection formData) : base()
        {
            this._File = _file;
            this.FormData = formData;
        }

        public override Stream InputStream {
            get
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(_Data);
                MemoryStream stream = new MemoryStream(byteArray);
                return stream;
            }
        }


        public override NameValueCollection Form
        {
            get
            {
                return FormData;
            }
        }

        public override HttpFileCollectionBase Files
        {
            get
            {
                return new HttpFileCollectionBaseTest(_File);
            }
        }
    }

    public class HttpFileCollectionBaseTest : HttpFileCollectionBase, ICollection, IEnumerable
    {
        public string _FileName;
        public Dictionary<string, HttpPostedFileBaseTest> _List = new Dictionary<string, HttpPostedFileBaseTest>();
        public HttpFileCollectionBaseTest()
        { }

        public HttpFileCollectionBaseTest(string file) 
        {
            this._FileName = file;
            this._List = new Dictionary<string, HttpPostedFileBaseTest>();
            this._List.Add("file", new HttpPostedFileBaseTest(file));
        }

        public override int Count
        {
            get { return _List.Count; }
        }

        public override IEnumerator GetEnumerator()
        {
            return _List.Keys.GetEnumerator();
        }

        public override HttpPostedFileBase this[string name]
        {
            get
            {
                return _List[name];
            }
        }

        
    }

    public class HttpPostedFileBaseTest : HttpPostedFileBase
    {
        public override string FileName { get; }
        public override string ContentType { get; }

        public override Stream InputStream
        {
            get
            {
                var rootPath = AppDomain.CurrentDomain.BaseDirectory;
                var testFile = Path.Combine(rootPath, FileName);
                return File.OpenRead(testFile);
            }
        }
        public HttpPostedFileBaseTest(string fileName) : base()
        {
            this.FileName = fileName;
            this.ContentType = "test";
        }
    }
}
