# Meeting Room Reservation

## Project Goals

- Deliver a demo-ready prototype in one day.
- Keep the product focused on two pages: `Room Search` and `My Reservations`.
- Enable one end-to-end user flow: search -> reserve -> view reservations -> cancel.

## Scope

### In Scope

- One hardcoded default user (no sign-up/login).
- Date-based room search and available-slot display.
- Reservation creation from room cards.
- My Reservations list and cancellation.
- Basic validation and clear success/failure feedback.

### Out of Scope

- Authentication/authorization features.
- Multi-user collaboration or approval workflow.
- Recurring bookings, waitlist, or external integrations.
- Admin tools, analytics, and enterprise-grade non-functional requirements.

## User Roles

- Default Test User
  - Searches rooms by date.
  - Creates reservations from available slots.
  - Views and cancels personal reservations.

## Core Features

1. Date-Based Room Search
- Default date is today.
- Only today/future dates are selectable.
- Changing the date automatically refreshes the room list.

2. Room Availability Cards
- Display rooms as cards (4 cards per row).
- Each card shows image, room name, room number, capacity, and available slots only.
- If no slots are available, show: `No reservable slots available.`

3. Reservation Creation (Modal)
- Triggered by `Reserve` from a room card.
- Modal includes date (auto-filled), slot picker, event name, notes, and confirm action.
- Selected summary updates in real time: start time, end time, duration.

4. Reservation Rules and Validation
- Reservable window per day: 08:00-18:00.
- Slot size: 30 minutes.
- Slots must be consecutive.
- Max reservation length: 2 hours (4 slots).
- Unavailable/conflicting slots must be rejected.

5. My Reservations Management
- List all reservations for the default user.
- Support cancellation per reservation.
- After cancellation, released slots become available again.

## Acceptance Criteria

1. On first load, date defaults to today and past dates cannot be selected.
2. When date changes, room list refreshes automatically.
3. Room cards show required room info and available slots only.
4. `Reserve` is shown only when a room still has available slots.
5. Reservation submission succeeds only for valid consecutive slots up to 4 slots.
6. Successful reservation appears in My Reservations; canceled reservation is removed and slots are released.
