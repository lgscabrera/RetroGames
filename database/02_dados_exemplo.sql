-- Etapa 4: dados de exemplo (execute depois do 01)
SET NAMES utf8mb4;

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
(1, 'Novo'), (2, 'Usado'), (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
(1, 'Administrador'), (2, 'Cliente');

INSERT INTO tb_plataforma (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
(1, 'Super Nintendo', 'Nintendo', 1990),
(2, 'Mega Drive', 'Sega', 1988),
(3, 'PlayStation', 'Sony', 1994),
(4, 'Nintendo 64', 'Nintendo', 1996),
(5, 'Atari 2600', 'Atari', 1977),
(6, 'Game Boy', 'Nintendo', 1989),
(7, 'Neo Geo', 'SNK', 1990);

INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
(1, 'Plataforma'), (2, 'Ação'), (3, 'RPG'), (4, 'Aventura'),
(5, 'Luta'), (6, 'Corrida'), (7, 'Tiro'), (8, 'Puzzle');

INSERT INTO tb_jogo (co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo) VALUES
(1,  'Super Mario World', 'Clássico de plataforma do Super Nintendo', 1990, 149.90, 3, TRUE,  UTC_TIMESTAMP(), 1, 2),
(2,  'The Legend of Zelda: A Link to the Past', NULL, 1991, 189.90, 2, TRUE,  UTC_TIMESTAMP(), 1, 2),
(3,  'Chrono Trigger', NULL, 1995, 399.90, 1, TRUE,  UTC_TIMESTAMP(), 1, 3),
(4,  'Street Fighter II Turbo', NULL, 1993, 129.90, 4, TRUE,  UTC_TIMESTAMP(), 1, 2),
(5,  'Super Metroid', NULL, 1994, 279.90, 1, TRUE,  UTC_TIMESTAMP(), 1, 2),
(6,  'Sonic the Hedgehog 2', NULL, 1992, 99.90, 5, TRUE,  UTC_TIMESTAMP(), 2, 2),
(7,  'Streets of Rage 2', NULL, 1992, 159.90, 2, TRUE,  UTC_TIMESTAMP(), 2, 2),
(8,  'Phantasy Star IV', NULL, 1993, 249.90, 1, TRUE,  UTC_TIMESTAMP(), 2, 3),
(9,  'Mortal Kombat II', NULL, 1994, 89.90, 0, TRUE,  UTC_TIMESTAMP(), 2, 2),
(10, 'Gran Turismo 2', NULL, 1999, 59.90, 4, TRUE,  UTC_TIMESTAMP(), 3, 2),
(11, 'Final Fantasy VII', NULL, 1997, 199.90, 2, TRUE,  UTC_TIMESTAMP(), 3, 2),
(12, 'Crash Bandicoot', NULL, 1996, 119.90, 3, TRUE,  UTC_TIMESTAMP(), 3, 1),
(13, 'Metal Gear Solid', NULL, 1998, 129.90, 1, TRUE,  UTC_TIMESTAMP(), 3, 2),
(14, 'Super Mario 64', NULL, 1996, 249.90, 2, TRUE,  UTC_TIMESTAMP(), 4, 2),
(15, 'GoldenEye 007', NULL, 1997, 229.90, 3, TRUE,  UTC_TIMESTAMP(), 4, 2),
(16, 'Mario Kart 64', NULL, 1996, 199.90, 4, TRUE,  UTC_TIMESTAMP(), 4, 2),
(17, 'Pitfall!', NULL, 1982, 79.90, 2, TRUE,  UTC_TIMESTAMP(), 5, 2),
(18, 'Tetris', NULL, 1989, 89.90, 5, TRUE,  UTC_TIMESTAMP(), 6, 2),
(19, 'Pokémon Red', NULL, 1998, 179.90, 2, TRUE,  UTC_TIMESTAMP(), 6, 2),
(20, 'Kirby''s Dream Land', NULL, 1992, 99.90, 1, FALSE, UTC_TIMESTAMP(), 6, 3);

INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
(1,1),(1,4),(2,4),(2,2),(3,3),(3,4),(4,5),(5,2),(5,4),(6,1),
(6,2),(7,2),(8,3),(9,5),(10,6),(11,3),(11,4),(12,1),(13,2),(13,4),
(14,1),(14,4),(15,7),(15,2),(16,6),(17,1),(18,8),(19,3),(19,4),(20,1);
