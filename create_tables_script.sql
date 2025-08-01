CREATE TABLE users (
    user_id BIGSERIAL PRIMARY KEY,
    dni VARCHAR(255) NOT NULL DEFAULT '',
    username VARCHAR(255) NOT NULL DEFAULT '',
    surname VARCHAR(255) DEFAULT '',
    email VARCHAR(255) NOT NULL DEFAULT '',
    friend_code VARCHAR(255) NOT NULL DEFAULT '',
    password BYTEA,
    role INTEGER NOT NULL DEFAULT 0,
    role_string VARCHAR(255) NOT NULL DEFAULT '',
    inscription_date TIMESTAMP NOT NULL DEFAULT '0001-01-01 00:00:00'::timestamp
);

CREATE TABLE user_friends (
    user_friend_id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    friend_id BIGINT NOT NULL,

    CONSTRAINT fk_user_friend_user
        FOREIGN KEY (user_id)
        REFERENCES users(user_id),

    CONSTRAINT fk_user_friend_friends
        FOREIGN KEY (friend_id)
        REFERENCES users(user_id)
);

CREATE TABLE routines (
    routine_id BIGSERIAL PRIMARY KEY,
    routine_name VARCHAR(255) NOT NULL DEFAULT '',
    routine_description VARCHAR(255) DEFAULT '',
    user_id BIGINT NOT NULL,

    CONSTRAINT fk_user_routine_routines
        FOREIGN KEY (user_id)
        REFERENCES users(user_id)
);

CREATE TABLE split_days (
    split_day_id BIGSERIAL PRIMARY KEY,
    day_name INTEGER NOT NULL,
    day_name_string VARCHAR(255) NOT NULL,
    routine_id BIGINT NOT NULL,
    day_exercises_description TEXT NOT NULL DEFAULT '',

    CONSTRAINT fk_split_days_routine
        FOREIGN KEY (routine_id)
        REFERENCES routines(routine_id)
);

CREATE TABLE exercises (
    exercise_id BIGSERIAL PRIMARY KEY,
    exercise_name VARCHAR(255) NOT NULL DEFAULT '',
    routine_id BIGINT NOT NULL,
    split_day_id BIGINT NOT NULL,

    CONSTRAINT fk_exercises_routine
        FOREIGN KEY (routine_id)
        REFERENCES routines(routine_id),

    CONSTRAINT fk_exercises_split_day
        FOREIGN KEY (split_day_id)
        REFERENCES split_days(split_day_id)
);

CREATE TABLE exercise_progress (
    progress_id BIGSERIAL PRIMARY KEY,
    exercise_id BIGINT NOT NULL,
    routine_id BIGINT NOT NULL,
    day_name VARCHAR(50) NOT NULL DEFAULT '',
    sets INTEGER NOT NULL,
    reps INTEGER NOT NULL,
    weight FLOAT NOT NULL,
    performed_at TIMESTAMP NOT NULL,

    CONSTRAINT fk_exercise_progress_exercise
        FOREIGN KEY (exercise_id)
        REFERENCES exercises(exercise_id),

    CONSTRAINT fk_exercise_progress_routine
        FOREIGN KEY (routine_id)
        REFERENCES routines(routine_id)
);