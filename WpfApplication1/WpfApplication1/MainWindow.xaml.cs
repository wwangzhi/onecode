﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    class CustomData
    {
        public long CreationTime;
        public int Name;
        public int ThreadNum;
    }
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts;
        public int i = 0;
        public MainWindow()
        {
            strList = SetUpURLList();
            InitializeComponent();
        }

        private void foo()
        {
            while (i<10)
            {
                Console.WriteLine($"i={i}");
                Thread.Sleep(TimeSpan.FromSeconds(4));
            }
        }


        private async void button_Click(object sender, RoutedEventArgs e)
        {
/*            Task[] taskArray = new Task[10];
            for (int i = 0; i < taskArray.Length; i++)
            {
                taskArray[i] = Task.Factory.StartNew((Object obj) =>
                {
                    CustomData data = obj as CustomData;
                    if (data == null)return;
                    data.ThreadNum = Thread.CurrentThread.ManagedThreadId;
                    Console.WriteLine("Task #{0} created at {1} on thread #{2}.",data.Name, data.CreationTime, data.ThreadNum);
                },new CustomData() {Name = i, CreationTime = DateTime.Now.Ticks});
            }
            Task.WaitAll(taskArray);
            */

            await Task.Run(()=>foo());

            resultsTextBox.Clear();

            // Instantiate the CancellationTokenSource.
            cts = new CancellationTokenSource();

            try
            {
                await AccessTheWebAsync(cts.Token);
                resultsTextBox.Text += "\r\nDownloads complete.";
            }
            catch (OperationCanceledException)
            {
                resultsTextBox.Text += "\r\nDownloads canceled.\r\n";
            }
            catch (Exception)
            {
                resultsTextBox.Text += "\r\nDownloads failed.\r\n";
            }

            cts = null;
        }


        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (cts != null)
            {
                cts.Cancel();
            }
        }


        async Task AccessTheWebAsync(CancellationToken ct)
        {

            // Make a list of web addresses.
            List<string> urlList = SetUpURLList();


/*            var tasks = Enumerable.Range(1, 5).Select(i => MakeTask(i,ct)).ToList();
            while (tasks.Count > 0)
            {
                // Identify the first task that completes.
                var firstFinishedTask = await Task.WhenAny(tasks);

                // ***Remove the selected task from the list so that you don't
                // process it more than once.
                tasks.Remove(firstFinishedTask);

                // Await the completed task.
                //int length = await firstFinishedTask;
                
                resultsTextBox.Text += $"\r\n  { firstFinishedTask.Result}";
            }
            */


            // ***Use ToList to execute the query and start the tasks. 
            // ***Create a query that, when executed, returns a collection of tasks.
            HttpClient client = new HttpClient();
            var downloadTasksQuery =from url in urlList select ProcessURL(url, client, ct);
            var downloadTasks = downloadTasksQuery.ToList();

            // ***Add a loop to process the tasks one at a time until none remain.
            while (downloadTasks.Count > 0)
            {
                // Identify the first task that completes.
                var firstFinishedTask = await Task.WhenAny(downloadTasks);

                // ***Remove the selected task from the list so that you don't
                // process it more than once.
                downloadTasks.Remove(firstFinishedTask);

                // Await the completed task.
                //int length = await firstFinishedTask;
                resultsTextBox.Text += $"\r\nLength of the download:  {firstFinishedTask.Result}";
            }
            
        }

        private async Task<string> MakeTask(int i,CancellationToken ct)
        {
            var managedThreadId = Thread.CurrentThread.ManagedThreadId;
            await Task.Delay(TimeSpan.FromSeconds(i), ct);

            return i+" : "+ managedThreadId+":"+ strList[i];
        }

        private IList<string> strList;

        private List<string>  SetUpURLList()
        {
            List<string> urls = new List<string>
            {
                "http://msdn.microsoft.com",
                "http://msdn.microsoft.com/library/windows/apps/br211380.aspx",
                "http://msdn.microsoft.com/en-us/library/hh290136.aspx",
                "http://msdn.microsoft.com/en-us/library/dd470362.aspx",
                "http://msdn.microsoft.com/en-us/library/aa578028.aspx",
                "http://msdn.microsoft.com/en-us/library/ms404677.aspx",
                "http://msdn.microsoft.com/en-us/library/ff730837.aspx"
            };
            return urls;
        }


        async Task<string> ProcessURL(string url, HttpClient client, CancellationToken ct)
        {
            // GetAsync returns a Task<HttpResponseMessage>. 
            HttpResponseMessage response = await client.GetAsync(url, ct);

            // Retrieve the website contents from the HttpResponseMessage.
            byte[] urlContents = await response.Content.ReadAsByteArrayAsync();
            var managedThreadId = Thread.CurrentThread.ManagedThreadId;

            //await Task.Delay(TimeSpan.FromSeconds(1), ct);

            return $"Id={managedThreadId}, Len={urlContents.Length}";
        }
    }
}
