using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace MultiLogViewer.Tests
{
    [TestClass]
    public class TestSetup
    {
        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            // .NET Core/.NET 5+ で Shift-JIS などのコードページエンコーディングをサポートするために登録
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
