create table if not exists "Infrastructure"."ToDoUser" (
    "UserId" uuid not null,
    "TelegramUserId" bigint not null,
    "TelegramUserName" text null,
    "RegisteredAt" timestamp not null default now(),
    constraint "PKToDoUser" primary key ("UserId")
    );

create unique index if not exists "UQ_ToDoUser_TelegramUserId" on "Infrastructure"."ToDoUser"("TelegramUserId");

create table if not exists "Infrastructure"."ToDoList" (
    "Id" uuid not null,
    "Name" text not null,
    "ToDoUserId" uuid not null,
    "CreatedAt" timestamp not null default now(),
    constraint "PK_ToDoList" primary key ("Id"),
    constraint "FK_ToDoList_ToDoUserId" foreign key ("ToDoUserId") references "ToDoUser"("UserId")
    );

create index if not exists "I_ToDoList_ToDoUserId" on "Infrastructure"."ToDoList"("ToDoUserId");

create table if not exists "Infrastructure"."ToDoItem" (
    "Id" uuid not null,
    "Name" text not null,
    "ToDoUserId" uuid not null,
    "ToDoListId" uuid null,
    "Deadline" timestamp not null,
    "State" integer not null,
    "CreatedAt" timestamp not null default now(),
    "StateChangedAt" timestamp null,
    constraint "PKToDoItem" primary key ("Id"),
    constraint "FK_ToDoItem_ToDoUserId" foreign key ("ToDoUserId") references "ToDoUser"("UserId"),
    constraint "FK_ToDoItem_ToDoListId" foreign key ("ToDoListId") references "ToDoList"("Id")
    );
create index if not exists "I_ToDoItem_ToDoUserId" on "Infrastructure"."ToDoItem"("ToDoUserId");
create index if not exists "I_ToDoItem_ToDoListId" on "Infrastructure"."ToDoItem"("ToDoListId");

commit;


