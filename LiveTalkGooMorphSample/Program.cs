/*
 * Copyright 2019 FUJITSU LIMITED
 * システム名：LiveTalkGooMorphSample
 * 概要      ：LiveTalk-gooラボ形態素解析API連携サンプルアプリ
*/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace LiveTalkGooMorphSample
{
    class Program
    {
        static LiveTalk.FileCollaboration FileInterface;
        static BlockingCollection<byte[]> AudioQueue = new BlockingCollection<byte[]>();
        static CancellationTokenSource TokenSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            var model = new Models.GooMorphModel();
            var param = new string[]
            {
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "LiveTalkOutput.csv"),
            };
            if (args.Length >= 1)
            {
                param[0] = args[0];
            }
            Console.WriteLine("InputCSVFileName  :" + param[0]);
            FileInterface = new LiveTalk.FileCollaboration(param[0], "");

            // ファイル入力(LiveTalk常時ファイル出力からの入力)
            FileInterface.RemoteMessageReceived += async (s) =>
            {
                var reg = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                var items = reg.Split(s);

                Console.WriteLine(">>>>>>>");
                Console.WriteLine($"DateTime:{items[0]}");
                Console.WriteLine($"Speaker:{items[1]}");
                Console.WriteLine($"Speech contents:{items[2]}");
                Console.WriteLine($"Translate content:{items[3]}");
                Console.WriteLine("↓");

                var item = items[3] == "\"\"" ? items[2] : items[3];
                item = item.Length >= 2 ? item.Substring(1, item.Length - 2) : string.Empty;
                (List<string> data, string errorMessage) = await model.TextToSpeechAsync(item);
                if (data != null)
                {
                    // 形態素表示
                    Console.WriteLine(string.Join<string>(",", data));
                }
                else
                {
                    // エラーメッセージ表示
                    Console.WriteLine(errorMessage);
                }
            };

            // ファイル監視開始
            if (System.IO.File.Exists(param[0]))
            {
                System.IO.File.Delete(param[0]);
            }
            FileInterface.WatchFileStart();

            // 処理終了待ち
            var message = Console.ReadLine();

            // ファイル監視終了
            TokenSource.Cancel(true);
            FileInterface.WatchFileStop();
        }
    }
}
