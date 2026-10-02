CREATE TABLE "comment"
(
    id         TEXT                     NOT NULL,
    article_id TEXT                     NOT NULL,
    message    TEXT,
    author     TEXT,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL,
);