-- Adminer 4.8.1 PostgreSQL 16.1 (Debian 16.1-1.pgdg120+1) dump

\connect "postgres";

DROP TABLE IF EXISTS "author_kafka";
DROP SEQUENCE IF EXISTS author_kafka_id_seq;
CREATE SEQUENCE author_kafka_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."author_kafka" (
    "id" integer DEFAULT nextval('author_kafka_id_seq') NOT NULL,
    "author_id" integer NOT NULL,
    "name" character varying(128) NOT NULL,
    "surname" character varying NOT NULL,
    CONSTRAINT "author_kafka_author_id" UNIQUE ("author_id"),
    CONSTRAINT "author_kafka_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "author_kafka";

DROP TABLE IF EXISTS "copyright";
DROP SEQUENCE IF EXISTS copyright_id_seq;
CREATE SEQUENCE copyright_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."copyright" (
    "id" integer DEFAULT nextval('copyright_id_seq') NOT NULL,
    "name" character varying(256) NOT NULL,
    CONSTRAINT "copyright_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "copyright";
INSERT INTO "copyright" ("name") VALUES
('Public Domain');

DROP TABLE IF EXISTS "genre";
DROP SEQUENCE IF EXISTS genre_id_seq;
CREATE SEQUENCE genre_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."genre" (
    "id" integer DEFAULT nextval('genre_id_seq') NOT NULL,
    "name" character varying(1024) NOT NULL,
    CONSTRAINT "genre_name" UNIQUE ("name"),
    CONSTRAINT "genre_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "genre";
INSERT INTO "genre" ("name") VALUES
('Sonatas'),
('Barcarolles'),
('For Piano'),
('Ballades');

DROP TABLE IF EXISTS "musical_score";
DROP SEQUENCE IF EXISTS musical_score_id_seq;
CREATE SEQUENCE musical_score_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."musical_score" (
    "id" integer DEFAULT nextval('musical_score_id_seq') NOT NULL,
    "title" character varying(1024) NOT NULL,
    "alias" character varying(1024),
    "year_of_composition" character varying(64),
    "description" character varying(2048),
    "opus" character varying(64),
    "author_id" integer NOT NULL,
    CONSTRAINT "musical_score_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "musical_score";
INSERT INTO "musical_score" ("title", "alias", "year_of_composition", "description", "opus", "author_id") VALUES
('Ballade No.3',	NULL,	'1840-41',	'1 ballad (Allegretto)',	'Op.47',	1),
('Sonata No.8',	'Pathétique',	'1798',	'3 movements',	'Op.13',	2),
('Nocturnes',	NULL,	'1836',	'2 pieces',	'Op.27',	1),
('Piano Quintet in A major',	'The troute',	'1819?',	'5 movements',	'D.667',	1),
('Symphony No.41 in C major',	'Jupiter',	'1788 (August 10)',	'4 movements',	'K.551',	3),
('Symphony No.9',	NULL,	'1822-1824',	'4 movements',	'Op.125',	4);

DROP TABLE IF EXISTS "pdf_file";
DROP SEQUENCE IF EXISTS pdf_file_id_seq;
CREATE SEQUENCE pdf_file_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."pdf_file" (
    "id" integer DEFAULT nextval('pdf_file_id_seq') NOT NULL,
    "musical_score_id" integer NOT NULL,
    "path" character varying(2048) NOT NULL,
    "upload_date" date NOT NULL,
    "publisher" character varying(512),
    "copyright_id" integer,
    "is_urtext" boolean DEFAULT false NOT NULL,
    "user_id" integer NOT NULL,
    "comments" character varying(2048) DEFAULT '',
    CONSTRAINT "pdf_file_path" UNIQUE ("path"),
    CONSTRAINT "pdf_file_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "pdf_file";
INSERT INTO "pdf_file" ("musical_score_id", "path", "upload_date", "publisher", "copyright_id", "is_urtext", "user_id", "comments") VALUES
(2,	'PdfScores/MSL_Y120biIeMSa6jTXYPhDYSQY_IMSLP86694-PMLP01648-chopin-ballade_no_3.pdf',	'2024-01-25',	'Leipzig: C.F. Peters',	1,	'f',	0,	'#41057: 600 dpi no page cleaning!'),
(3,	'PdfScores/MSL_SvaG64K4eVBCopUQy8Fpb4H_IMSLP03865-Beethoven_-_Piano_Sonatas_Lamond_-_8.pdf',	'2024-01-26',	'Klaviersonaten, Band I Berlin: Ullstein, n.d.(ca.1918). Reissue - Leipzig: Breitkopf und Härtel, 1923. Plate 28726.',	1,	'f',	0,	NULL),
(3,	'PdfScores/MSL_QFZHh6AszsDfgmUQNkeuPrZ_IMSLP894991-PMLP1410-Sonata_No._8.pdf',	'2024-01-26',	'	Louis Köhler (1820-1886) Adolf Ruthardt (1849-1934)',	1,	'f',	0,	'	scanned at 600 dpi (for title pages see Piano Sonata No.1, Op.2 No.1), original papersize and borders. If you rate this score, please give feedback to scanner talk'),
(5,	'PdfScores/MSL_Z5ohEghBgJQItLZBepeXOAt_IMSLP751817-PMLP1573-00_MOZART_SYM_41_in_C_minor,_K551_(Jupiter)_-_Score.pdf',	'2024-01-28',	'Howard Chandler Robbins Landon (1926-2009)',	1,	't',	0,	NULL);

DROP TABLE IF EXISTS "score_genre_relationship";
DROP SEQUENCE IF EXISTS score_genre_relationship_id_seq;
CREATE SEQUENCE score_genre_relationship_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."score_genre_relationship" (
    "id" integer DEFAULT nextval('score_genre_relationship_id_seq') NOT NULL,
    "score_id" integer NOT NULL,
    "genre_id" integer NOT NULL,
    CONSTRAINT "score_genre_relationship_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "score_genre_relationship_score_id_genre_id" UNIQUE ("score_id", "genre_id")
) WITH (oids = false);

TRUNCATE "score_genre_relationship";
INSERT INTO "score_genre_relationship" ("score_id", "genre_id") VALUES
(2,	4);

DROP TABLE IF EXISTS "user_kafka";
DROP SEQUENCE IF EXISTS user_kafka_id_seq;
CREATE SEQUENCE user_kafka_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1;

CREATE TABLE "public"."user_kafka" (
    "id" integer DEFAULT nextval('user_kafka_id_seq') NOT NULL,
    "user_id" integer NOT NULL,
    "username" character varying(128) NOT NULL,
    "name" character varying(128),
    "surname" character varying(128),
    CONSTRAINT "user_kafka_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "user_kafka_user_id" UNIQUE ("user_id")
) WITH (oids = false);

TRUNCATE "user_kafka";

ALTER TABLE ONLY "public"."pdf_file" ADD CONSTRAINT "pdf_file_copyright_id_fkey" FOREIGN KEY (copyright_id) REFERENCES copyright(id) ON UPDATE CASCADE ON DELETE SET NULL NOT DEFERRABLE;
ALTER TABLE ONLY "public"."pdf_file" ADD CONSTRAINT "pdf_file_musical_score_id_fkey" FOREIGN KEY (musical_score_id) REFERENCES musical_score(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."score_genre_relationship" ADD CONSTRAINT "score_genre_relationship_genre_id_fkey" FOREIGN KEY (genre_id) REFERENCES genre(id) ON UPDATE CASCADE ON DELETE SET NULL NOT DEFERRABLE;
ALTER TABLE ONLY "public"."score_genre_relationship" ADD CONSTRAINT "score_genre_relationship_score_id_fkey" FOREIGN KEY (score_id) REFERENCES musical_score(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;

-- 2024-01-30 12:41:27.372652+00