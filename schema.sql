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
  pub_date bigint,
  flagged integer
);

create table latest (
  id serial primary key,
  command_id integer not null
);

-- The below schema is found by the following session library: https://github.com/leonibr/community-extensions-cache-postgres/blob/0d7237679d4d706cfcd097adcbed6075772ab557/Extensions.Caching.PostgreSql/SqlCommands.cs#L14-L26
create table if not exists "session"
(
  "Id" text COLLATE pg_catalog."default" NOT NULL,
  "Value" bytea,
  "ExpiresAtTime" timestamp with time zone,
  "SlidingExpirationInSeconds" double precision,
  "AbsoluteExpiration" timestamp with time zone,
  CONSTRAINT "DistCache_pkey" PRIMARY KEY ("Id")
);
