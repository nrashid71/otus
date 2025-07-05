create table if not exists "Infrastructure"."ToDoUser" (
    "UserId" uuid primary key not null,
    "TelegramUserId" bigint not null,
    "TelegramUserName" varchar(250) null,
    "RegisteredAt" timestamp not null default now());

create unique index if not exists "IToDoUserTelegramUserId" on "Infrastructure"."ToDoUser"("TelegramUserId");

create table if not exists "Infrastructure"."ToDoList" (
    "Id" uuid primary key not null,
    "Name" varchar(250) not null,
    "ToDoUser" uuid references "ToDoUser"("UserId") not null,
    "CreatedAt" timestamp not null default now()
    );

create index if not exists "IToDoListToDoUser" on "Infrastructure"."ToDoList"("ToDoUser");

create table if not exists "Infrastructure"."ToDoItem" (
    "Id" uuid primary key not null,
    "Name" varchar(250) not null,
    "ToDoUser" uuid references "ToDoUser"("UserId") not null,
    "ToDoList" uuid references "ToDoList"("Id") null,
    "Deadline" timestamp not null,
    "State" integer not null,
    "CreatedAt" timestamp not null default now(),
    "StateChangedAt" timestamp null
    );
create index if not exists "IToDoItemToDoUser" on "Infrastructure"."ToDoItem"("ToDoUser");
create index if not exists "IToDoItemToDoList" on "Infrastructure"."ToDoItem"("ToDoList");

commit;


