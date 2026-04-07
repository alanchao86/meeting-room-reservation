# DB Schema

本文件定義 Meeting Room Reservation System prototype 的核心資料模型與 schema，提供一致的資料基準。

## 資料表 / Schema

### 1 時段表示方式（共用規則）

- 每日可預約範圍固定為 `08:00 - 18:00`。
- 以 30 分鐘為 1 個 slot，共 20 個 slot。
- 使用 `slot_index` 表示時段位置：


| slot_index | 時間區間    |
| ------------ | ------------- |
| 0          | 08:00-08:30 |
| 1          | 08:30-09:00 |
| ...        | ...         |
| 19         | 17:30-18:00 |

- `start_slot_index` 採含起點。
- `end_slot_index` 採不含終點（exclusive）。
- `slot_index=20` 不代表實際 slot，不需存資料列，僅作為 `end_slot_index` 的邊界值（代表結束於 `18:00`）。
- 例如
  - `start_slot_index=2, end_slot_index=6` 代表 `09:00-11:00`（4 slots, 2 小時）。
  - 最後一格 `17:30-18:00` 以 `start_slot_index=19, end_slot_index=20` 表示。

### 2 `users`

用途：目前僅儲存預設測試使用者（無登入流程）。


| 欄位         | 型別     | 必填 | 約束 | 說明                                        |
| -------------- | ---------- | ------ | ------ | --------------------------------------------- |
| id           | string   | 是   | PK   | 使用者 ID，prototype 固定可為`default-user` |
| display_name | string   | 是   |      | 顯示名稱，例如`Default Test User`           |
| is_default   | boolean  | 是   |      | 標示是否為預設測試使用者                    |
| created_at   | datetime | 是   |      | 建立時間                                    |

### 3 `rooms`

用途：會議室主檔，提供 Room Search 與 Room Cards 顯示資料。


| 欄位        | 型別     | 必填 | 約束   | 說明         |
| ------------- | ---------- | ------ | -------- | -------------- |
| id          | string   | 是   | PK     | 會議室 ID    |
| room_name   | string   | 是   |        | 會議室名稱   |
| room_number | string   | 是   | UNIQUE | 會議室編號   |
| capacity    | integer  | 是   | > 0    | 可容納人數   |
| image_url   | string   | 否   |        | 卡片圖片連結 |
| created_at  | datetime | 是   |        | 建立時間     |
| updated_at  | datetime | 是   |        | 更新時間     |

### 4 `reservations`

用途：儲存預約資料，支援建立預約、查詢我的預約、取消預約。


| 欄位             | 型別     | 必填 | 約束           | 說明                                  |
| ------------------ | ---------- | ------ | ---------------- | --------------------------------------- |
| id               | string   | 是   | PK             | 預約 ID                               |
| user_id          | string   | 是   | FK -> users.id | 預約人（prototype 固定 default user） |
| room_id          | string   | 是   | FK -> rooms.id | 被預約會議室                          |
| reservation_date | date     | 是   |                | 預約日期                              |
| start_slot_index | integer  | 是   | 0~19           | 起始 slot（含）                       |
| end_slot_index   | integer  | 是   | 1~20           | 結束 slot（不含）                     |
| event_name       | string   | 是   |                | 事件名稱                              |
| created_at       | datetime | 是   |                | 建立時間                              |
| updated_at       | datetime | 是   |                | 更新時間                              |

約束：


| 規則                                              | 說明                       |
| --------------------------------------------------- | ---------------------------- |
| `start_slot_index < end_slot_index`               | 預約區段必須有正長度       |
| `(end_slot_index - start_slot_index) <= 4`        | 單筆最長 4 slots（2 小時） |
| `start_slot_index >= 0` 且 `end_slot_index <= 20` | 不可超出 08:00-18:00 範圍  |

## 實體關聯


| 關聯                          | 類型   | 說明                               |
| ------------------------------- | -------- | ------------------------------------ |
| `users` 1 -> N `reservations` | 一對多 | 一位使用者可有多筆預約             |
| `rooms` 1 -> N `reservations` | 一對多 | 一間會議室可被多次預約（不同時段） |
