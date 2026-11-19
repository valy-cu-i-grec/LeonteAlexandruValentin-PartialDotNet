# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

R5 (rezervare in trecut): in starter nu exista deloc. Acum e in Booking.Create, in Domain/Bookings/Booking.cs. Ora vine din TimeProvider prin CreateBookingHandler.


## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

Motiv: "camera e ocupata" e un rezultat normal al business-ului, nu o situatie neasteptata. Semnatura Result<T> arata ca operatia poate esua, iar exceptia nu apare in
semnatura.
In starter: erau BusinessException si ExceptionMiddleware, care le transforma in ProblemDetails.
Ce se intampla acum: CreateBookingHandler intoarce BookingErrors.RoomOccupied. Endpoint-ul face result.Error.ToProblem() (Common/ErrorHttpExtensions.cs). Tipul Conflict devine
409, title este codul erorii, si nu se apeleaza SaveChangesAsync.


## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

Stiu: RoomHubDbContext, RoomConfiguration, BookingConfiguration, UtcDateTimeConverter, RoomRepository, BookingRepository si DatabaseInitializer, toate in
Infrastructure/Persistence. Mai stie si Program.cs, prin AddDbContext/UseSqlite. 
Castig: handler-ele pot fi testate cu un fals scris de mana, iar schimbarea bazei de date afecteaza un singur folder.
Cost: mai multe fisiere si interfete. Repository-urile raman mici, pentru ca am adaugat doar metodele necesare (GetByIdAsync, Add, ListActiveForRoomAsync,
CountActiveForBookerOnDayAsync).

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

Program.cs (MapBookings) -> BookingsEndpoints -> CreateBookingEndpoint.Handle
-> ICommandHandler<CreateBookingCommand,CreateBookingResult> = CreateBookingHandler
-> IRoomRepository (RoomRepository)         404 daca camera lipseste
-> Booking.Create (DateRange.Create)        R2-R6, R10
-> IBookingRepository (BookingRepository)   ListActiveForRoomAsync -> ConflictsWith (R7)
                                            CountActiveForBookerOnDayAsync (R9)
-> bookings.Add -> IUnitOfWork.SaveChangesAsync (RoomHubDbContext) -> SQLite roomhub.db
<- Result: Error -> ToProblem()  |  succes -> TypedResults.Created

## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

Nu am respins bucati de cod, doar le-am verificat. Am pus la indoiala si am cerut explicatii pentru fiecare implementare pentru a ma asigua de corectitudine.

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

As adauga teste automate, si as face R7 si R9 sa functioneze cu tranzactie