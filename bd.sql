-- ============================================================
-- USERS
-- ============================================================

CREATE TABLE users (
    id TEXT PRIMARY KEY,
    username TEXT NOT NULL,
    avatar_url TEXT,
    created_at TIMESTAMP DEFAULT NOW(),
    last_login TIMESTAMP
);

-- ============================================================
-- PHOTOS (Modo PhotoSweep)
-- ============================================================

CREATE TABLE photos (
    id TEXT PRIMARY KEY,
    user_id TEXT NOT NULL,
    uri TEXT NOT NULL,
    date_taken TIMESTAMP,
    keep_status BOOLEAN,
    reviewed_at TIMESTAMP
);

-- ============================================================
-- ROOMS (Modo PvP PhotoClash)
-- ============================================================

CREATE TABLE rooms (
    id TEXT PRIMARY KEY,
    code TEXT NOT NULL,
    status TEXT DEFAULT 'waiting',
    rounds_total INTEGER,
    seconds_per_round INTEGER,
    nsfw_allowed BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================
-- ROOM PLAYERS (Jugadores dentro de cada sala)
-- ============================================================

CREATE TABLE room_players (
    id TEXT PRIMARY KEY,
    room_id TEXT NOT NULL,
    user_id TEXT NOT NULL,
    joined_at TIMESTAMP DEFAULT NOW(),
    score INTEGER DEFAULT 0
);

-- ============================================================
-- ROUNDS (Rondas del modo PvP)
-- ============================================================

CREATE TABLE rounds (
    id TEXT PRIMARY KEY,
    room_id TEXT NOT NULL,
    round_number INTEGER NOT NULL,
    prompt_phrase TEXT NOT NULL,
    started_at TIMESTAMP,
    finished_at TIMESTAMP
);

-- ============================================================
-- ROUND PHOTOS (Fotos enviadas para cada ronda)
-- ============================================================

CREATE TABLE round_photos (
    id TEXT PRIMARY KEY,
    round_id TEXT NOT NULL,
    player_id TEXT NOT NULL,
    photo_url TEXT NOT NULL,
    uploaded_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================
-- VOTES (Votos de cada jugador)
-- ============================================================

CREATE TABLE votes (
    id TEXT PRIMARY KEY,
    round_id TEXT NOT NULL,
    voter_player_id TEXT NOT NULL,
    voted_player_id TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================
-- MATCH RESULTS (Ganador final)
-- ============================================================

CREATE TABLE match_results (
    id TEXT PRIMARY KEY,
    room_id TEXT NOT NULL,
    winner_player_id TEXT NOT NULL,
    total_rounds INTEGER,
    created_at TIMESTAMP DEFAULT NOW()
);

-- ============================================================
-- APP SETTINGS (Preferencias del usuario)
-- ============================================================

CREATE TABLE app_settings (
    id TEXT PRIMARY KEY,
    user_id TEXT NOT NULL,
    dark_mode BOOLEAN DEFAULT FALSE,
    notifications BOOLEAN DEFAULT TRUE,
    language TEXT DEFAULT 'es'
);
