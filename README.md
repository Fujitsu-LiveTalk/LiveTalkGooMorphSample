# LiveTalkGooMorphSample
LiveTalk常時ファイル出力で出力したテキストを、gooラボ 形態素解析API を使って形態素に分解するサンプルです。  
発話内容の解析を行うための前処理などに活用できます。  
本サンプルコードは、.NET Core 3.0で作成しています。コードレベルでは.NET Framework 4.6と互換性があります。

![Process](https://github.com/FujitsuSSL-LiveTalk/LiveTalkGooMorphSample/blob/images/README.png)

# サンプルコードの動き
サンプルコード動作を簡単に説明すると次のような動作をします。  
1. LiveTalkで音声認識した結果がファイルに出力されるので、それを自動的に読込み、形態素解析APIを呼び出します。
2. 形態素解析APIから戻ってきた形態素をカンマ区切りで表示します。


# 事前準備
1. GitHubアカウントで [API利用登録ページ](https://labs.goo.ne.jp/jp/apiregister/) で登録し、アプリケーションIDを取得します。
2. 取得したアプリケーションIDをAPIキーとしてサンプルコードに指定します。
4. インターネットとの接続がPROXY経由の場合、PROXYサーバーや認証情報を設定してください。

# 連絡事項
本ソースコードは、LiveTalkの保守サポート範囲に含まれません。  
頂いたissueについては、必ずしも返信できない場合があります。  
LiveTalkそのものに関するご質問は、公式WEBサイトのお問い合わせ窓口からご連絡ください。
