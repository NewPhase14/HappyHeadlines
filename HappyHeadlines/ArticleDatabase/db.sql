CREATE TABLE article
(
    id         TEXT                     NOT NULL PRIMARY KEY,
    title      TEXT                     NOT NULL,
    content    TEXT                     NOT NULL,
    continent  TEXT                     NOT NULL,
    draft      BOOLEAN                  NOT NULL,
    created_at TIMESTAMP with time zone NOT NULL,
);