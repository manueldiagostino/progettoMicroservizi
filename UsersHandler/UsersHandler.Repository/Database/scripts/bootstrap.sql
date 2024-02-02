-- Adminer 4.8.1 PostgreSQL 16.1 (Debian 16.1-1.pgdg120+1) dump

\connect "postgres";

DROP TABLE IF EXISTS "bio";
DROP SEQUENCE IF EXISTS bio_id_seq;
CREATE SEQUENCE bio_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 2 CACHE 1;

CREATE TABLE "public"."bio" (
    "id" integer DEFAULT nextval('bio_id_seq') NOT NULL,
    "text" text,
    "user_id" integer NOT NULL,
    CONSTRAINT "bio_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "bio";
INSERT INTO "bio" ("id", "text", "user_id") VALUES
(1,	'Just some text for my bio!',	1),
(2,	'...',	2);

DROP TABLE IF EXISTS "transactional_outbox";
DROP SEQUENCE IF EXISTS transactional_outbox_id_seq;
CREATE SEQUENCE transactional_outbox_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 3 CACHE 1;

CREATE TABLE "public"."transactional_outbox" (
    "id" integer DEFAULT nextval('transactional_outbox_id_seq') NOT NULL,
    "table" character varying(64) NOT NULL,
    "message" character varying(2048) NOT NULL,
    CONSTRAINT "transactional_outbox_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "transactional_outbox";

DROP TABLE IF EXISTS "user";
DROP SEQUENCE IF EXISTS user_id_seq;
CREATE SEQUENCE user_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."user" (
    "id" integer DEFAULT nextval('user_id_seq') NOT NULL,
    "username" character varying(50) NOT NULL,
    "name" character varying(50),
    "surname" character varying(50),
    "propic_path" character varying(2048),
    "upload_time" bigint NOT NULL,
    "bio_id" integer,
    "salt" character varying(2048) NOT NULL,
    "hash" character varying(2048) NOT NULL,
    CONSTRAINT "user_id_bio" UNIQUE ("bio_id"),
    CONSTRAINT "user_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "user_username" UNIQUE ("username")
) WITH (oids = false);

TRUNCATE "user";
INSERT INTO "user" ("username", "name", "surname", "propic_path", "upload_time", "bio_id", "salt", "hash") VALUES
('manuel2001',	'Manuel',	'Di Agostino',	'ProfilePictures/MSL_5DEY9jy3duxirF4HgVQIb0R_man-avatar-profile-vector.jpg',	1706041700,	1,	'h5G/JhFQfS0eGy2UmjhN3Q==',	'Kp3Hnh9eIkhmIZfp5sJSXJ9qWiOGwcWdiq5kQ8j17LI='),
('_linux_00_',	NULL,	NULL,	'ProfilePictures/MSL_jrpEnnxiF422sfE4YfKeJRi_avatar-png-pic-male-avatar-icon.png',	1706094311,	2,	'8/9DAURNdAvJ8OnywvngOw==',	'xjh7LEFoAZFfVJpw88QhfNg0xAFhfTr8EkQckMUMGmg='),
('pianoLegend',	NULL,	NULL,	NULL,	1706885205,	NULL,	'zAWUNHypEgDuQK5jJv3lSA==',	'PiQojwGuxiFjOqgid+PcCCB+KUeW06e15/LPJEA+NWo='),
('ViolinStudent',	'Sara',	'Rossi',	'ProfilePictures/MSL_kk3xDibQHiL25ITyaPzFfVI_woman-with-violin-avatar.jpg',	1706885364,	NULL,	'PMRqDOAsydBylP/nJlMXVw==',	'ha4wjnetaRjT5uSspcsSTgOKtehFAJ8oxtS49JTRflI=');

ALTER TABLE ONLY "public"."bio" ADD CONSTRAINT "bio_id_user_fkey" FOREIGN KEY (user_id) REFERENCES "user"(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."user" ADD CONSTRAINT "user_id_bio_fkey" FOREIGN KEY (bio_id) REFERENCES bio(id) ON UPDATE CASCADE ON DELETE SET NULL NOT DEFERRABLE;

-- 2024-02-02 14:52:04.055824+00