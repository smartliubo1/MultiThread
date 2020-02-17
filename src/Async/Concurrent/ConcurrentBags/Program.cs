using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcurrentBags
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Task task = RunProgram();
            task.Wait();
        }

        static Dictionary<string, string[]> _contentEmulation = new Dictionary<string, string[]>();

        static async Task RunProgram()
        {
            var bag = new ConcurrentBag<CrawlingTask>();
            string[] urls = new[] { "http://microsoft.com/", "http://baidu.com/", "https://github.com/", "https://angular.io/" };

            var crawlers = new Task[4];

            for (int i = 1; i <= 4; i++)
            {
                string crawlerName = "Crawler" + i.ToString();
                bag.Add( new CrawlingTask { UrlToCrawl=urls[i-1],ProducerName="root"});
                crawlers[i - 1] = Task.Run(() => Crawl(bag, crawlerName));
            }
            await Task.WhenAll(crawlers);
        }

        static async Task Crawl(ConcurrentBag<CrawlingTask> bag,
            string crawlerName)
        {
            CrawlingTask task;

            while (bag.TryTake(out task))
            {
                IEnumerable<string> urls = await GetLinksFromContent(task);

                if (urls !=null)
                {
                    foreach (var url in urls)
                    {
                        var t = new CrawlingTask { UrlToCrawl = url, ProducerName = crawlerName };
                        bag.Add(t);
                    }
                }
                Console.WriteLine($"Indexing url {task.UrlToCrawl} posted by {task.ProducerName} is completed by {crawlerName}");
            }

        }

        static async Task<IEnumerable<string>> GetLinksFromContent(CrawlingTask crawlingTask)
        {
            await GetRandomDelay();
            if (_contentEmulation.ContainsKey(crawlingTask.UrlToCrawl))
            {
                return _contentEmulation[crawlingTask.UrlToCrawl];
            }
            return null;
        }

        static void CreateLinks()
        {
            _contentEmulation["http://microsoft.com/"] = new[] { 
                "http://microsoft.com/a.html" ,
                "http://microsoft.com/b.html" };
            _contentEmulation["http://microsoft.com/a.html"] = new[] {
                "http://microsoft.com/c.html" ,
                "http://microsoft.com/d.html" };
            _contentEmulation["http://microsoft.com/b.html"] = new[] {
                "http://microsoft.com/e.html" };

            _contentEmulation["http://baidu.com/"] = new[] {
                "http://baidu.com/a.html" ,
                "http://baidu.com/b.html" };
            _contentEmulation["http://baiducom/a.html"] = new[] {
                "http://baidu.com/c.html" ,
                "http://baidu.com/d.html" };
            _contentEmulation["http://baidu.com/b.html"] = new[] {
                "http://baidu.com/e.html" };

            _contentEmulation["https://github.com/"] = new[] {
                "https://github.com/a.html" ,
                "https://github.com/b.html" };
            _contentEmulation["https://github.com/a.html"] = new[] {
                "https://github.com/c.html" ,
                "https://github.com/d.html" };
            _contentEmulation["https://github.com/b.html"] = new[] {
                "https://github.com/e.html" };

            _contentEmulation["https://angular.io/"] = new[] {
                "https://angular.io/a.html",
                "https://angular.io/b.html",
                "https://angular.io/c.html"
            };
            _contentEmulation["https://angular.io/a.html"] = new[] {
                "https://angular.io/d.html",
                "https://angular.io/e.html",
                "https://angular.io/f.html"
            };

            _contentEmulation["https://angular.io/b.html"] = new[] {
                "https://angular.io/h.html",
                "https://angular.io/h.html",
                "https://angular.io/i.html"
            };


            _contentEmulation["https://angular.io/c.html"] = new[] {
                "https://angular.io/m.html",
                "https://angular.io/n.html",
                "https://angular.io/o.html"
            };

            _contentEmulation["https://angular.io/h.html"] = new[] {
                "https://angular.io/x.html",
                "https://angular.io/y.html",
                "https://angular.io/z.html"
            };
        }

        static Task GetRandomDelay()
        {
            int delay = new Random(DateTime.Now.Millisecond).Next(1, 500);
            return Task.Delay(delay);
        }

        class CrawlingTask
        {
            public string UrlToCrawl { get; set; }

            public string ProducerName { get; set; }
        }
    }
}
