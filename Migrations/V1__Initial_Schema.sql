CREATE DATABASE IF NOT EXISTS TodoApp;

USE TodoApp;

CREATE TABLE IF NOT EXISTS TodoItems (
    Id VARCHAR(36) NOT NULL PRIMARY KEY,
    Title VARCHAR(255) NOT NULL,
    Description TEXT,
    IsCompleted BOOLEAN NOT NULL DEFAULT FALSE,
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_created_at (CreatedAt),
    INDEX idx_is_completed (IsCompleted)
);

