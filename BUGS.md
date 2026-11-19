# BUGS

The starter contains behaviors that contradict `ASSIGNMENT.md`. Write here every discrepancy you found.
Fill it in yourself, in your own words. There is no required number: what counts is that they are real.

| Which rule from ASSIGNMENT.md is violated (R1–R8) | Where the problem was in the starter (file, approximate line) | How you discovered it (test, reading, AI) | What you changed |
|---|---|---|---|
| rezervarea in trecut (R5) | Services/BookingService.cs, metoda Create (liniile 8-45) | AI | Acum: Domain/Bookings/Booking.cs, metoda Booking.Create. Verificarea range.StartUtc < nowUtc intoarce BookingErrors.StartInPast (400). |
| durata maxima de 4 ore (R6) | Services/BookingService.cs, BookingService.Create (linia 17) | AI | Acum: tot in Booking.Create. Constanta Booking.MaxDuration e 4 ore, iar range.Duration > MaxDuration intoarce BookingErrors.TooLong (400). |
| intervalele care se ating sunt tratate ca suprapuse (R7) | Services/BookingService.cs, BookingService.cs, linia 28 | AI | Acum: Domain/Bookings/DateRange.cs, metoda Overlaps. Formula e StartUtc < other.EndUtc && other.StartUtc < EndUtc, cu < strict, in loc de <= din starter. |
| rezervarile anulate inca blocheaza intervalul (R7/R8) | Services/BookingService.cs, BookingService.cs, liniile 26-30 | AI | _(fill in)_ |
| anularea repetata nu da 409 (R8) | Services/BookingService.cs, BookingService.cs, metoda Cancel (liniile 56-60) | AI |Acum: Booking.Cancel() intoarce BookingErrors.AlreadyCancelled (409) daca statusul e deja Cancelled. Altfel trece rezervarea in Cancelled. |
