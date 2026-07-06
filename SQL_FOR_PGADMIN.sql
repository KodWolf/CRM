-- ============================================================================
-- SQL СКРИПТЫ ДЛЯ СОЗДАНИЯ ТАБЛИЦ CRM СИСТЕМЫ
-- База данных: CRMbank
-- СУБД: PostgreSQL
-- Скопируй весь текст и вставь в pgAdmin Query Tool
-- ============================================================================

-- 1. ТАБЛИЦА STATUS (Статусы обращений)
CREATE TABLE status (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) UNIQUE NOT NULL,
    description TEXT,
    "order" INT NOT NULL,
    is_active BOOLEAN DEFAULT TRUE
);

-- 2. ТАБЛИЦА PRIORITY (Приоритеты)
CREATE TABLE priority (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    level INT NOT NULL,
    color_code VARCHAR(7)
);

-- 3. ТАБЛИЦА CHANNEL (Каналы)
CREATE TABLE channel (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE NOT NULL,
    description TEXT,
    is_active BOOLEAN DEFAULT TRUE
);

-- 4. ТАБЛИЦА CLIENT (Клиенты)
CREATE TABLE client (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    middle_name VARCHAR(100),
    iin VARCHAR(12) UNIQUE NOT NULL,
    phone_number VARCHAR(20) NOT NULL,
    email VARCHAR(255),
    address TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN DEFAULT TRUE
);

-- 5. ТАБЛИЦА OPERATOR (Операторы)
CREATE TABLE operator (
    id SERIAL PRIMARY KEY,
    login VARCHAR(50) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    role VARCHAR(50) NOT NULL,
    department VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    last_login_at TIMESTAMP
);

-- 6. ТАБЛИЦА TICKET (Обращения)
CREATE TABLE ticket (
    id SERIAL PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    description TEXT NOT NULL,
    client_id INT NOT NULL,
    operator_id INT,
    status_id INT NOT NULL,
    priority_id INT NOT NULL,
    channel_id INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    closed_at TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (client_id) REFERENCES client(id) ON DELETE CASCADE,
    FOREIGN KEY (operator_id) REFERENCES operator(id) ON DELETE SET NULL,
    FOREIGN KEY (status_id) REFERENCES status(id),
    FOREIGN KEY (priority_id) REFERENCES priority(id),
    FOREIGN KEY (channel_id) REFERENCES channel(id)
);

-- 7. ТАБЛИЦА MESSAGE (Сообщения)
CREATE TABLE message (
    id SERIAL PRIMARY KEY,
    text TEXT NOT NULL,
    is_from_client BOOLEAN NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ticket_id INT NOT NULL,
    operator_id INT,
    FOREIGN KEY (ticket_id) REFERENCES ticket(id) ON DELETE CASCADE,
    FOREIGN KEY (operator_id) REFERENCES operator(id) ON DELETE SET NULL
);

-- 8. ТАБЛИЦА ATTACHMENT (Прикрепленные файлы)
CREATE TABLE attachment (
    id SERIAL PRIMARY KEY,
    file_name VARCHAR(255) NOT NULL,
    file_path TEXT NOT NULL,
    file_size BIGINT NOT NULL,
    file_type VARCHAR(100) NOT NULL,
    uploaded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ticket_id INT NOT NULL,
    message_id INT,
    FOREIGN KEY (ticket_id) REFERENCES ticket(id) ON DELETE CASCADE,
    FOREIGN KEY (message_id) REFERENCES message(id) ON DELETE CASCADE
);

-- 9. ТАБЛИЦА TICKET_HISTORY (История изменений)
CREATE TABLE ticket_history (
    id SERIAL PRIMARY KEY,
    ticket_id INT NOT NULL,
    field_name VARCHAR(100) NOT NULL,
    old_value TEXT,
    new_value TEXT,
    changed_by VARCHAR(100) NOT NULL,
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ticket_id) REFERENCES ticket(id) ON DELETE CASCADE
);

-- ============================================================================
-- СОЗДАНИЕ ИНДЕКСОВ
-- ============================================================================

CREATE INDEX idx_ticket_client_id ON ticket(client_id);
CREATE INDEX idx_ticket_operator_id ON ticket(operator_id);
CREATE INDEX idx_ticket_status_id ON ticket(status_id);
CREATE INDEX idx_ticket_created_at ON ticket(created_at);
CREATE INDEX idx_message_ticket_id ON message(ticket_id);
CREATE INDEX idx_message_created_at ON message(created_at);
CREATE INDEX idx_attachment_ticket_id ON attachment(ticket_id);
CREATE INDEX idx_ticket_history_ticket_id ON ticket_history(ticket_id);
CREATE INDEX idx_ticket_history_changed_at ON ticket_history(changed_at);

-- ============================================================================
-- ЗАПОЛНЕНИЕ СПРАВОЧНИКОВ
-- ============================================================================

-- Заполнение таблицы STATUS
INSERT INTO status (name, description, "order", is_active) VALUES
('New', 'Новое обращение', 1, true),
('InProgress', 'В обработке', 2, true),
('WaitingClient', 'Ожидание клиента', 3, true),
('Resolved', 'Решено', 4, true),
('Closed', 'Закрыто', 5, true);

-- Заполнение таблицы PRIORITY
INSERT INTO priority (name, level, color_code) VALUES
('Low', 1, '#4CAF50'),
('Medium', 2, '#FFC107'),
('High', 3, '#FF9800'),
('Critical', 4, '#F44336');

-- Заполнение таблицы CHANNEL
INSERT INTO channel (name, description, is_active) VALUES
('CallCenter', 'Обращение через колл-центр', true),
('Chat', 'Обращение через чат', true),
('Email', 'Обращение через email', true),
('MobileApp', 'Обращение через мобильное приложение', true),
('Website', 'Обращение через веб-сайт', true);

-- ============================================================================
-- ТЕСТОВЫЕ ДАННЫЕ (ОПЦИОНАЛЬНО)
-- ============================================================================

-- Добавление тестового клиента
INSERT INTO client (first_name, last_name, iin, phone_number, email, address, is_active) VALUES
('Иван', 'Петров', '900101300001', '+77012345678', 'ivan@mail.com', 'Алматы', true);

-- Добавление тестового оператора (пароль: testpass123 - хеш BCrypt)
-- Примечание: Замените password_hash на реальный хеш, полученный из приложения
INSERT INTO operator (login, password_hash, first_name, last_name, role, department, is_active) VALUES
('operator1', '$2a$12$xxx...xxx', 'Алексей', 'Сидоров', 'Operator', 'Support', true);

-- ============================================================================
-- ПРОВЕРКА СОЗДАННЫХ ТАБЛИЦ
-- ============================================================================

-- Посмотри все таблицы
SELECT table_name FROM information_schema.tables 
WHERE table_schema = 'public' ORDER BY table_name;

-- Посмотри все справочные данные
SELECT 'STATUS' as table_name, COUNT(*) as count FROM status
UNION ALL
SELECT 'PRIORITY', COUNT(*) FROM priority
UNION ALL
SELECT 'CHANNEL', COUNT(*) FROM channel;
