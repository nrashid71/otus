INSERT INTO "Infrastructure"."ToDoUser" ("UserId","TelegramUserId","TelegramUserName","RegisteredAt") VALUES
	 ('8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,1168793986,'rashid_ng','2025-06-01 12:43:34'),
	 ('c7cabccb-d348-4ce5-b98b-5cd2dd0e90fc'::uuid,1333693555,'test_user','2011-01-01 17:12:05');

INSERT INTO "Infrastructure"."ToDoList" ("Id","Name","ToDoUserId","CreatedAt") VALUES
	 ('0657708a-f671-49c0-b521-015b64e839b4'::uuid,'list1','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,'2025-06-14 17:12:43.056887'),
	 ('65e2149a-1fb7-4ebe-a661-0b008d9b2f71'::uuid,'list2','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,'2025-06-15 10:23:27.518036');

INSERT INTO "Infrastructure"."ToDoItem" ("Id","Name","ToDoUserId","ToDoListId","Deadline","State","CreatedAt","StateChangedAt") VALUES
	 ('10b590fb-ea5e-4361-917b-001d755ec00a'::uuid,'task_11','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,'0657708a-f671-49c0-b521-015b64e839b4'::uuid,'2026-01-01 00:00:00',0,'2025-06-15 11:38:40.374588',NULL),
	 ('18fab405-59a8-43da-8a63-a5bb2afbc42a'::uuid,'task_5','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,NULL,'2026-01-01 00:00:00',0,'2025-06-14 22:25:05.253384',NULL),
	 ('31d98c71-579f-40c8-acdb-8956d46c673d'::uuid,'task_2','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,NULL,'2026-01-01 00:00:00',0,'2025-06-14 22:23:38.360212',NULL),
	 ('8641464a-db94-49ce-a1c2-f3a22f24dacc'::uuid,'task_12','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,'0657708a-f671-49c0-b521-015b64e839b4'::uuid,'2026-01-01 00:00:00',0,'2025-06-15 11:39:05.319099',NULL),
	 ('bf4e9664-0062-42b7-a529-94f133ce886d'::uuid,'task_1','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,NULL,'2026-01-01 00:00:00',0,'2025-06-14 22:23:21.848774',NULL),
	 ('cc125c49-5d94-462a-82f1-85bb17e39b25'::uuid,'task_10','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,'0657708a-f671-49c0-b521-015b64e839b4'::uuid,'2026-01-01 00:00:00',0,'2025-06-15 11:38:11.746536',NULL),
	 ('ecc1914a-623b-4d38-a517-d554fd71df44'::uuid,'task_6','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,NULL,'2026-01-01 00:00:00',0,'2025-06-14 22:25:21.979617',NULL),
	 ('f60e6bfc-3993-4c3c-8793-016ae439a274'::uuid,'task_7','8e5d0b51-1244-45f4-ad56-0f7528c13d55'::uuid,NULL,'2026-01-01 00:00:00',0,'2025-06-14 22:25:37.015021',NULL);

COMMIT;
