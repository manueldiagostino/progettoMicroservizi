-- Adminer 4.8.1 PostgreSQL 16.1 (Debian 16.1-1.pgdg120+1) dump

\connect "postgres";

DROP TABLE IF EXISTS "copyright";
DROP SEQUENCE IF EXISTS copyright_id_seq;
CREATE SEQUENCE copyright_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 2 CACHE 1;

CREATE TABLE "public"."copyright" (
    "id" integer DEFAULT nextval('copyright_id_seq') NOT NULL,
    "name" character varying(256) NOT NULL,
    CONSTRAINT "copyright_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "copyright";
INSERT INTO "copyright" ("id", "name") VALUES
(1,	'Public Domain');

DROP TABLE IF EXISTS "genre";
DROP SEQUENCE IF EXISTS genre_id_seq;
CREATE SEQUENCE genre_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 5 CACHE 1;

CREATE TABLE "public"."genre" (
    "id" integer DEFAULT nextval('genre_id_seq') NOT NULL,
    "name" character varying(1024) NOT NULL,
    CONSTRAINT "genre_name" UNIQUE ("name"),
    CONSTRAINT "genre_pkey" PRIMARY KEY ("id")
) WITH (oids = false);

TRUNCATE "genre";
INSERT INTO "genre" ("id", "name") VALUES
(1,	'Sonatas'),
(2,	'Barcarolles'),
(3,	'For Piano'),
(4,	'Ballades');

DROP TABLE IF EXISTS "musical_score";
DROP SEQUENCE IF EXISTS musical_score_id_seq;
CREATE SEQUENCE musical_score_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 4 CACHE 1;

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
INSERT INTO "musical_score" ("id", "title", "alias", "year_of_composition", "description", "opus", "author_id") VALUES
(2,	'Ballade No. 3',	NULL,	'1840-41',	'1 ballad (Allegretto)',	'Op. 47',	1),
(3,	'Sonata No. 8',	'Pathétique',	'1798',	'three movements',	'Op. 13',	2);

DROP TABLE IF EXISTS "pdf_file";
DROP SEQUENCE IF EXISTS pdf_file_id_seq;
CREATE SEQUENCE pdf_file_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 5 CACHE 1;

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
INSERT INTO "pdf_file" ("id", "musical_score_id", "path", "upload_date", "publisher", "copyright_id", "is_urtext", "user_id", "comments") VALUES
(1,	2,	'PdfScores/MSL_Y120biIeMSa6jTXYPhDYSQY_IMSLP86694-PMLP01648-chopin-ballade_no_3.pdf',	'2024-01-25',	'Leipzig: C.F. Peters',	1,	'f',	0,	'#41057: 600 dpi no page cleaning!'),
(3,	3,	'PdfScores/MSL_SvaG64K4eVBCopUQy8Fpb4H_IMSLP03865-Beethoven_-_Piano_Sonatas_Lamond_-_8.pdf',	'2024-01-26',	'Klaviersonaten, Band I Berlin: Ullstein, n.d.(ca.1918). Reissue - Leipzig: Breitkopf und Härtel, 1923. Plate 28726.',	1,	'f',	0,	NULL),
(4,	3,	'PdfScores/MSL_QFZHh6AszsDfgmUQNkeuPrZ_IMSLP894991-PMLP1410-Sonata_No._8.pdf',	'2024-01-26',	'	Louis Köhler (1820-1886) Adolf Ruthardt (1849-1934)',	1,	'f',	0,	'	scanned at 600 dpi (for title pages see Piano Sonata No.1, Op.2 No.1), original papersize and borders. If you rate this score, please give feedback to scanner talk');

DROP TABLE IF EXISTS "score_genre_relationship";
DROP SEQUENCE IF EXISTS score_genre_relationship_id_seq;
CREATE SEQUENCE score_genre_relationship_id_seq INCREMENT 1 MINVALUE 1 MAXVALUE 2147483647 START 3 CACHE 1;

CREATE TABLE "public"."score_genre_relationship" (
    "id" integer DEFAULT nextval('score_genre_relationship_id_seq') NOT NULL,
    "score_id" integer NOT NULL,
    "genre_id" integer NOT NULL,
    CONSTRAINT "score_genre_relationship_pkey" PRIMARY KEY ("id"),
    CONSTRAINT "score_genre_relationship_score_id_genre_id" UNIQUE ("score_id", "genre_id")
) WITH (oids = false);

TRUNCATE "score_genre_relationship";
INSERT INTO "score_genre_relationship" ("id", "score_id", "genre_id") VALUES
(2,	2,	4);

ALTER TABLE ONLY "public"."pdf_file" ADD CONSTRAINT "pdf_file_copyright_id_fkey" FOREIGN KEY (copyright_id) REFERENCES copyright(id) ON UPDATE CASCADE ON DELETE SET NULL NOT DEFERRABLE;
ALTER TABLE ONLY "public"."pdf_file" ADD CONSTRAINT "pdf_file_musical_score_id_fkey" FOREIGN KEY (musical_score_id) REFERENCES musical_score(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;

ALTER TABLE ONLY "public"."score_genre_relationship" ADD CONSTRAINT "score_genre_relationship_genre_id_fkey" FOREIGN KEY (genre_id) REFERENCES genre(id) ON UPDATE CASCADE ON DELETE SET NULL NOT DEFERRABLE;
ALTER TABLE ONLY "public"."score_genre_relationship" ADD CONSTRAINT "score_genre_relationship_score_id_fkey" FOREIGN KEY (score_id) REFERENCES musical_score(id) ON UPDATE CASCADE ON DELETE CASCADE NOT DEFERRABLE;

-- 2024-01-26 10:28:42.45697+00