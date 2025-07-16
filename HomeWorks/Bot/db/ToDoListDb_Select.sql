-- GetByGuid(Guid id, CancellationToken ct)
select "Id", "Name", "ToDoUserId", "ToDoListId", "Deadline", "State", "CreatedAt", "StateChangedAt" from "ToDoItem" tdi where tdi."Id" = '10b590fb-ea5e-4361-917b-001d755ec00a'::uuid;

-- GetAllByUserId(Guid userId, CancellationToken ct)
select "Id", "Name", "ToDoUserId", "ToDoListId", "Deadline", "State", "CreatedAt", "StateChangedAt" from "ToDoItem" tdi where tdi."ToDoUserId" = '8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid;

-- GetActiveByUserId(Guid userId, CancellationToken ct)
select "Id", "Name", "ToDoUserId", "ToDoListId", "Deadline", "State", "CreatedAt", "StateChangedAt" from "ToDoItem" tdi where tdi."ToDoUserId" = '8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid and "State" = 0;

-- ExistsByName(Guid userId, string name, CancellationToken ct) -- 0 == Нет записей с указанным именем, 1 == Есть запись с указанным именем
select count(*) as "ExistByName" where exists(select * from "ToDoItem" tdi where tdi."ToDoUserId" = '8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid and "Name" = 'task_7');

-- CountActive(Guid userId, CancellationToken ct)
select count(*) as "CountActive" from "ToDoItem" tdi where tdi."ToDoUserId" = '8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid and "State" = 0;

