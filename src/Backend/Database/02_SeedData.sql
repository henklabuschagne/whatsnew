-- =============================================
-- Seed Data for What's New Application
-- =============================================

USE WhatsNewDB;
GO

-- =============================================
-- Seed Users
-- =============================================
-- Password for both users: "Password123!" (hashed with BCrypt)
-- In production, use proper password hashing
IF NOT EXISTS (SELECT * FROM Users WHERE Email = 'viewer@example.com')
BEGIN
    INSERT INTO Users (UserId, Name, Email, PasswordHash, Role)
    VALUES 
        (NEWID(), 'John Viewer', 'viewer@example.com', '$2a$11$XYZ...', 'viewer'),
        (NEWID(), 'Admin User', 'admin@example.com', '$2a$11$XYZ...', 'admin');
END
GO

-- =============================================
-- Seed Tags (Module Tags)
-- =============================================
IF NOT EXISTS (SELECT * FROM Tags)
BEGIN
    INSERT INTO Tags (TagId, Label, Value, Type)
    VALUES 
        (NEWID(), 'Import', 'import', 'module'),
        (NEWID(), 'Export', 'export', 'module'),
        (NEWID(), 'Packs', 'packs', 'module'),
        (NEWID(), 'Systems', 'systems', 'module'),
        (NEWID(), 'Security', 'security', 'module'),
        (NEWID(), 'Reports', 'reports', 'module'),
        (NEWID(), 'Publisher', 'publisher', 'module'),
        (NEWID(), 'Dashboard', 'dashboard', 'module');
END
GO

PRINT 'Seed data inserted successfully';
GO
