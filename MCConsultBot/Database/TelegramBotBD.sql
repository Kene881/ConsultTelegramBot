create database EFIntroductionDb;

use EFIntroductionDb
go

create table Room 
(
	id int primary key,
	num int
)

create table Schedule
(
	id int primary key,
	time_receipt varchar(30)
)

create table Doctors
(
	id int primary key,
	first_name varchar(50),
	last_name varchar(50),
	price int,
	id_room int,
	id_schedule int,
	FOREIGN KEY (id_room)  REFERENCES Room (id),
	foreign key (id_schedule) references Schedule (id)
)



create table Cities 
(
	id int primary key,
	name_city varchar(100)
)

create table Addresses
(
	id int primary key,
	place varchar(200),
	id_city int,
	foreign key (id_city) references Cities (id)
)

create table RateLimiterTb
(
	chatId varchar(100),
	Counter int
)

--insert into RateLimiterTb values ('hello', 0);

--UPDATE RateLimiterTb SET Counter = Counter + 1 WHERE chatId = 'hello';

--select * from RateLimiterTb;

--delete from RateLimiterTb;

--use master

--drop database EFIntroductionDb

--"������� �������", "������������ �����", "������� ���"

--dictionary["������� �������"] = "10:00-17:00";
--dictionary["������������ �����"] = "09:00-15:00";
--dictionary["������� ���"] = "15:00-19:00";

--dictionary["������� �������"] = "232";
--dictionary["������������ �����"] = "236";
--dictionary["������� ���"] = "105";

--2) Doctors (id, first name, last name, id_room, id_sched) Doctors - Room = ������ � ������;
--3) Room (id, num)
--4) Schedule (id, time) Schedule - Doctors = ������ � ������.

--5) Cities (id, name)
--6) Addresses (id, place, id_city)



insert into Room values(1, 232), (2, 236), (3, 105);
insert into Schedule values(1, '10:00-17:00'), (2, '09:00-15:00'), (3, '15:00-19:00')

select * from Room;
select * from Schedule;

insert into Doctors values (1, '�������', '�������', 200, 1, 1), (2, '������������', '�����', 300, 2, 2), (3, '�������', '���', 400, 3, 3)

select Doctors.first_name, Doctors.last_name, Doctors.price, Schedule.time_receipt, Room.num from Doctors 
join Room on Room.id = Doctors.id_room
join Schedule on Schedule.id = Doctors.id_schedule;

select Schedule.time_receipt from Doctors
join Schedule on Schedule.id = Doctors.id_schedule
where Doctors.first_name = '�������';

select Room.num from Doctors 
join Room on Room.id = Doctors.id_room where Doctors.first_name = '�������';

select Doctors.price from Doctors where Doctors.first_name = '�������';


-- 0000000000000000000000000000000000000000
select * from Cities;
--"������", "������", "���������"
insert into Cities values (1, '������'), (2, '������'), (3, '���������')

--map["������"] = new string[] { "�������� 176", "���� 98" };
--map["������"] = new string[] { "��������� 21" };
--map["���������"] = new string[] { "���� 11" };

select * from Addresses;
insert into Addresses values (1, '�������� 176', 1), (2, '���� 98', 1), (3, '��������� 21', 2), (4, '���� 11', 3);

select Cities.name_city from Cities 
join Addresses on Cities.id = Addresses.id_city 
where Cities.name_city = '������';
