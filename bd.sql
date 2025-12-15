-- ============================================================
-- EXTENSIONES NECESARIAS
-- ============================================================

CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- ============================================================
-- USERS
-- ============================================================

CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    username VARCHAR(50) NOT NULL,
    avatar_url TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    last_login TIMESTAMP
);

-- ============================================================
-- PHOTOS (Modo PhotoSweep)
-- ============================================================

CREATE TABLE photos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL,
    uri TEXT NOT NULL,
    date_taken TIMESTAMP,
    keep_status BOOLEAN,
    reviewed_at TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE INDEX idx_photos_user ON photos(user_id);

-- ============================================================
-- ROOMS (Modo PvP PhotoClash)
-- ============================================================

CREATE TABLE rooms (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(10) UNIQUE NOT NULL,
    status VARCHAR(20) DEFAULT 'waiting',
    rounds_total INT NOT NULL CHECK (rounds_total > 0),
    seconds_per_round INT NOT NULL CHECK (seconds_per_round > 0),
    nsfw_allowed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================
-- ROOM PLAYERS (Jugadores dentro de cada sala)
-- ============================================================

CREATE TABLE room_players (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    room_id UUID NOT NULL,
    user_id UUID NOT NULL,
    joined_at TIMESTAMP DEFAULT NOW(),
    score INT DEFAULT 0,
    FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    UNIQUE (room_id, user_id)
);

CREATE INDEX idx_room_players_room ON room_players(room_id);

-- ============================================================
-- ROUNDS (Rondas del modo PvP)
-- ============================================================

CREATE TABLE rounds (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    room_id UUID NOT NULL,
    round_number INT NOT NULL,
    prompt_phrase TEXT NOT NULL,
    started_at TIMESTAMP,
    finished_at TIMESTAMP,
    FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE,
    UNIQUE (room_id, round_number)
);

-- ============================================================
-- ROUND PHOTOS (Fotos enviadas para cada ronda)
-- ============================================================

CREATE TABLE round_photos (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    round_id UUID NOT NULL,
    player_id UUID NOT NULL,
    photo_url TEXT NOT NULL,
    uploaded_at TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY (round_id) REFERENCES rounds(id) ON DELETE CASCADE,
    FOREIGN KEY (player_id) REFERENCES room_players(id) ON DELETE CASCADE,
    UNIQUE (round_id, player_id)
);

CREATE INDEX idx_round_photos_round ON round_photos(round_id);

-- ============================================================
-- VOTES (Votos de cada jugador)
-- ============================================================

CREATE TABLE votes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    round_id UUID NOT NULL,
    voter_player_id UUID NOT NULL,
    voted_player_id UUID NOT NULL,
    created_at TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY (round_id) REFERENCES rounds(id) ON DELETE CASCADE,
    FOREIGN KEY (voter_player_id) REFERENCES room_players(id) ON DELETE CASCADE,
    FOREIGN KEY (voted_player_id) REFERENCES room_players(id) ON DELETE CASCADE,
    CHECK (voter_player_id <> voted_player_id),
    UNIQUE (round_id, voter_player_id)
);

CREATE INDEX idx_votes_round ON votes(round_id);

-- ============================================================
-- MATCH RESULTS (Ganador final)
-- ============================================================

CREATE TABLE match_results (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    room_id UUID NOT NULL UNIQUE,
    winner_player_id UUID NOT NULL,
    total_rounds INT,
    created_at TIMESTAMP DEFAULT NOW(),
    FOREIGN KEY (room_id) REFERENCES rooms(id) ON DELETE CASCADE,
    FOREIGN KEY (winner_player_id) REFERENCES room_players(id) ON DELETE SET NULL
);

-- ============================================================
-- APP SETTINGS (Preferencias del usuario)
-- ============================================================

CREATE TABLE app_settings (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL UNIQUE,
    dark_mode BOOLEAN DEFAULT FALSE,
    notifications BOOLEAN DEFAULT TRUE,
    language VARCHAR(10) DEFAULT 'es',
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);
