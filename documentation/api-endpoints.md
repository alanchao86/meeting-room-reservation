# API Design

## 目的

紀錄系統當前 REST API

## 設計總覽

### API 範圍


| 模組                                             | API                                         |
| -------------------------------------------------- | --------------------------------------------- |
| Date-Based Room Search + Room Availability Cards | `GET /api/rooms?date=YYYY-MM-DD`            |
| Reservation Creation (Modal)                     | `POST /api/reservations`                    |
| My Reservations Management                       | `GET /api/reservations`                     |
| My Reservations Management（取消）               | `DELETE /api/reservations/{reservation_id}` |

### 規則

- 目前無登入流程；後端以固定 `default-user` 作為操作主體。
- 時段規則固定：`08:00-18:00`、30 分鐘一格、單筆預約最多 4 slots（2 小時）。

### API資料欄位表示

- `slot_index`：0-19，對應 08:00~18:00，每30分鐘 1 slot。
- `end_slot_index` 設計為 exclusive。

## API 設計

### 1 取得會議室與可預約時段

**Method**

- `GET`

**Path**

- `/api/rooms`

**Purpose**

- 依日期回傳首頁 Room Search 所需資料，包含每個 room 的可預約時段。

**Request parameters**


| 參數 | 位置  | 型別                | 必填 | 說明                   |
| ------ | ------- | --------------------- | ------ | ------------------------ |
| date | query | string (YYYY-MM-DD) | 是   | 查詢日期（今天或未來） |

**Response body（200）**

```json
{
  "date": "2026-04-06",
  "rooms": [
    {
      "room_id": "room-a",
      "room_name": "Orchid",
      "room_number": "A-301",
      "capacity": 8,
      "image_url": "https://example.com/rooms/a301.jpg",
      "available_slots": [
        { "slot_index": 0, "start_time": "08:00", "end_time": "08:30" },
        { "slot_index": 1, "start_time": "08:30", "end_time": "09:00" },
        { "slot_index": 2, "start_time": "09:00", "end_time": "09:30" }
      ],
      "has_reservable_slots": true
    }
  ]
}
```

**Success cases**

- `200 OK`：成功回傳 rooms 與各 room 的 `available_slots`。
- 若某 room 無可預約時段時，`available_slots` 為空陣列，`has_reservable_slots=false`。

**Error cases**


| HTTP Status                 | Error Code        | Message                              | 說明                  |
| ----------------------------- | ------------------- | -------------------------------------- | ----------------------- |
| `400 Bad Request`           | `INVALID_REQUEST` | `Invalid or missing date parameter.` | `date` 缺失或格式錯誤 |
| `422 Unprocessable Entity`  | `INVALID_DATE`    | `Date cannot be earlier than today.` | `date` 早於今天       |
| `500 Internal Server Error` | `INTERNAL_ERROR`  | `Unexpected server error.`           | 系統錯誤              |

**Business rules**

- 僅回傳指定日期可預約 slot，不回傳不可預約 slot。
- 可預約 slot 會排除已被 reservation 佔用的時段。

**Validation rules**

- `date` 必須符合 `YYYY-MM-DD`。
- `date` 不可早於今天。

### 4.2 建立預約

**Method**

- `POST`

**Path**

- `/api/reservations`

**Purpose**

- 建立一筆新的會議室預約

**Request body**


| 欄位             | 型別                | 必填 | 說明              |
| ------------------ | --------------------- | ------ | ------------------- |
| room_id          | string              | 是   | 目標會議室 ID     |
| date             | string (YYYY-MM-DD) | 是   | 預約日期          |
| start_slot_index | integer             | 是   | 起始 slot（包含） |
| end_slot_index   | integer             | 是   | 結束 slot（不含） |
| event_name       | string              | 是   | 事件名稱          |

**Request body example**

```json
{
  "room_id": "room-a",
  "date": "2026-04-06",
  "start_slot_index": 2,
  "end_slot_index": 6,
  "event_name": "Team Meeting"
}
```

**Response body（201）**

```json
{
  "reservation_id": "resv-1001",
  "user_id": "default-user",
  "room": {
    "room_id": "room-a",
    "room_name": "Orchid",
    "room_number": "A-301"
  },
  "date": "2026-04-06",
  "start_slot_index": 2,
  "end_slot_index": 6,
  "start_time": "09:00",
  "end_time": "11:00",
  "duration_slots": 4,
  "duration_minutes": 120,
  "event_name": "Team Meeting",
  "created_at": "2026-04-05T09:30:00Z"
}
```

**Success cases**

- `201 Created`：建立成功並回傳新預約。

**Error cases**


| HTTP Status                 | Error Code           | Message                                                    | 說明                                                             |
| ----------------------------- | ---------------------- | ------------------------------------------------------------ | ------------------------------------------------------------------ |
| `400 Bad Request`           | `INVALID_REQUEST`    | `Invalid request body.`                                    | 必要欄位缺失或格式不合法                                         |
| `404 Not Found`             | `ROOM_NOT_FOUND`     | `Room not found.`                                          | `room_id` 不存在                                                 |
| `409 Conflict`              | `SLOT_CONFLICT`      | `Selected slots are no longer available. Please reselect.` | 時段已被佔用（建立當下衝突）                                     |
| `422 Unprocessable Entity`  | `INVALID_SLOT_RANGE` | `Please select consecutive slots up to 2 hours.`           | 違反預約規則（非連續、超過 4 slots、日期過去、超出 08:00-18:00） |
| `500 Internal Server Error` | `INTERNAL_ERROR`     | `Unexpected server error.`                                 | 系統錯誤                                                         |

**Business rules**

- 預約日期必須為今天或未來。
- 時段必須落在 08:00-18:00 對應的 slot 範圍內。
- 每次預約上限4小時，`end_slot_index - start_slot_index` 必須介於 1 到 4。
- 同一 room、同一天預約 不可有重疊時段。

**Validation rules**

- `event_name` 必填且不可為空字串。
- `start_slot_index`、`end_slot_index` 必須為整數。
- `start_slot_index < end_slot_index`。
- `start_slot_index >= 0` 且 `end_slot_index <= 20`。

### 4.3 取得預約清單 My Reservation

**Method**

- `GET`

**Path**

- `/api/reservations`

**Purpose**

- 回傳 default user 的所有預約，供 My Reservations 頁面顯示。

**Request parameters**

- None
- 日後時做其他user後會修改此處

**Response body（200）**

```json
{
  "reservations": [
    {
      "reservation_id": "resv-1001",
      "event_name": "Team Meeting",
      "date": "2026-04-06",
      "start_slot_index": 2,
      "end_slot_index": 6,
      "start_time": "09:00",
      "end_time": "11:00",
      "duration_slots": 4,
      "duration_minutes": 120,
      "room": {
        "room_id": "room-a",
        "room_name": "Orchid",
        "room_number": "A-301"
      }
    }
  ]
}
```

**Success cases**

- `200 OK`：成功回傳清單。
- 無資料時 `reservations` 回傳空陣列。

**Error cases**


| HTTP Status                 | Error Code       | Message                    | 說明     |
| ----------------------------- | ------------------ | ---------------------------- | ---------- |
| `500 Internal Server Error` | `INTERNAL_ERROR` | `Unexpected server error.` | 系統錯誤 |

**Business rules**

- 僅回傳 default user 自己的資料。
- 依 `date`、`start_slot_index` desc 排序。

**Validation rules**

- 無額外輸入驗證。

### 4.4 取消預約

**Method**

- `DELETE`

**Path**

- `/api/reservations/{reservation_id}`

**Purpose**

- 取消指定預約，並釋放該時段。

**Request parameters**


| 參數           | 位置 | 型別   | 必填 | 說明    |
| ---------------- | ------ | -------- | ------ | --------- |
| reservation_id | path | string | 是   | 預約 ID |

**Response body（200）**

```json
{
  "reservation_id": "resv-1001",
  "status": "canceled",
  "message": "Reservation canceled successfully."
}
```

**Success cases**

- `200 OK`：取消成功。

**Error cases**


| HTTP Status                 | Error Code              | Message                    | 說明                                |
| ----------------------------- | ------------------------- | ---------------------------- | ------------------------------------- |
| `404 Not Found`             | `RESERVATION_NOT_FOUND` | `Reservation not found.`   | 找不到該預約，或不屬於 default user |
| `500 Internal Server Error` | `INTERNAL_ERROR`        | `Unexpected server error.` | 系統錯誤                            |

**Business rules**

- 取消成功後，該筆資料不應再出現在 My Reservations。
- 對應 room/date 的 slots 應可再次被預約。

**Validation rules**

- `reservation_id` 必須存在且格式合法。

## 5. Error Handling Convention

目前統一採最小錯誤格式：`code + message`。

```json
{
  "error": {
    "code": "SLOT_CONFLICT",
    "message": "Selected slots are no longer available. Please reselect."
  }
}
```

## 6. HTTP Status Code


| Status Code               | 使用情境                                       |
| --------------------------- | ------------------------------------------------ |
| 200 OK                    | 查詢成功、取消成功                             |
| 201 Created               | 建立預約成功                                   |
| 400 Bad Request           | 請求格式錯誤、缺少必要欄位                     |
| 404 Not Found             | room 或 reservation 不存在                     |
| 409 Conflict              | 預約時段衝突                                   |
| 422 Unprocessable Entity  | 規則驗證失敗（過去日期、超時段、超過 4 slots） |
| 500 Internal Server Error | 系統錯誤                                       |
