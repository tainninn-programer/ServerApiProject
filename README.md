# ServerApiProject

## 概要

ASP.NET Core（Minimal API）、EF Core、SQLiteで作った、アイテム（id と name）のCRUD API。
CRUD操作とDB接続の基礎を学ぶための学習用プロジェクト。

## 実装したAPI

| メソッド | パス | 内容 | 成功 | 失敗 |
| --- | --- | --- | --- | --- |
| GET | `/items` | 全件取得 | 200 | — |
| GET | `/items/{id}` | idで1件取得 | 200 | 404（見つからない）、400（idが数字でない） |
| POST | `/items` | 追加 | 201（`Location` に作成先） | 400（nameが空） |
| PATCH | `/items/{id}` | 名前の変更 | 204 | 400（nameが空）、404（見つからない） |
| DELETE | `/items/{id}` | 削除 | 204 | 404（見つからない） |

## 設計判断

（書きかけ）

## 動かし方

（書きかけ）

## できていないこと・展望

（書きかけ）
