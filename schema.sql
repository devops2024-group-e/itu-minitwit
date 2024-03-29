create table "user" (
  user_id serial primary key,
  username varchar(100) not null,
  email varchar(300) not null,
  pw_hash varchar(200) not null
);

create table follower (
  who_id integer,
  whom_id integer,
  PRIMARY KEY (who_id, whom_id)
);

create table message (
  message_id serial primary key,
  author_id integer not null,
  text text not null,
  pub_date integer,
  flagged integer
);

create table latest (
  id serial primary key,
  command_id integer not null
);
