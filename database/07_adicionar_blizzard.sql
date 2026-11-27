-- Etapa de desafio: jogos da Blizzard Entertainment
SET NAMES utf8mb4;

INSERT IGNORE INTO tb_plataforma
    (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES (8, 'PC', 'Blizzard Entertainment', 1991);

INSERT IGNORE INTO tb_genero (no_genero) VALUES
    ('MMORPG'), ('Estratégia'), ('MOBA'), ('Cartas');

INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
SELECT dados.no_jogo, dados.ds_jogo, dados.ano, dados.preco, dados.estoque, TRUE,
       UTC_TIMESTAMP(), 8, 1
FROM (
    SELECT 'World of Warcraft: Midnight' AS no_jogo,
           'Atual expansão de World of Warcraft, na saga Worldsoul.' AS ds_jogo,
           2026 AS ano, 149.90 AS preco, 10 AS estoque
) AS dados
WHERE NOT EXISTS (
    SELECT 1 FROM tb_jogo WHERE tb_jogo.no_jogo = dados.no_jogo
);

INSERT INTO tb_jogo
    (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo,
     dt_cadastro, co_plataforma, co_condicao_jogo)
SELECT dados.no_jogo, dados.ds_jogo, dados.ano, dados.preco, dados.estoque, TRUE,
       UTC_TIMESTAMP(), 8, 1
FROM (
    SELECT 'Warcraft: Orcs & Humans' AS no_jogo, 'RTS original da Blizzard.' AS ds_jogo, 1994 AS ano, 79.90 AS preco, 5 AS estoque UNION ALL
    SELECT 'Warcraft II: Tides of Darkness', 'Segundo jogo da série Warcraft.', 1995, 89.90, 5 UNION ALL
    SELECT 'Warcraft III: Reign of Chaos', 'RTS com campanha de Azeroth e heróis.', 2002, 119.90, 6 UNION ALL
    SELECT 'Warcraft III: The Frozen Throne', 'Expansão de Warcraft III.', 2003, 99.90, 6 UNION ALL
    SELECT 'Diablo', 'Action RPG original de Tristram.', 1996, 89.90, 5 UNION ALL
    SELECT 'Diablo II', 'Aventura de ação em quatro atos sombrios.', 2000, 109.90, 5 UNION ALL
    SELECT 'Diablo II: Lord of Destruction', 'Expansão de Diablo II.', 2001, 99.90, 5 UNION ALL
    SELECT 'Diablo III', 'Action RPG de luta contra demônios e Lúcifer.', 2012, 129.90, 8 UNION ALL
    SELECT 'Diablo III: Reaper of Souls', 'Expansão com a campanha da Morte.', 2014, 79.90, 6 UNION ALL
    SELECT 'Diablo IV', 'Quarta aventura principal de Diablo.', 2023, 249.90, 8 UNION ALL
    SELECT 'StarCraft', 'Estratégia em tempo real com três povos.', 1998, 99.90, 6 UNION ALL
    SELECT 'StarCraft: Brood War', 'Expansão oficial de StarCraft.', 1998, 89.90, 5 UNION ALL
    SELECT 'StarCraft II: Wings of Liberty', 'Primeira campanha de StarCraft II.', 2010, 119.90, 6 UNION ALL
    SELECT 'StarCraft II: Heart of the Swarm', 'Campanha de Kerrigan em StarCraft II.', 2013, 99.90, 5 UNION ALL
    SELECT 'StarCraft II: Legacy of the Void', 'Campanha dos Protoss em StarCraft II.', 2015, 99.90, 5 UNION ALL
    SELECT 'Overwatch', 'Hero shooter da Blizzard.', 2016, 149.90, 8 UNION ALL
    SELECT 'Overwatch 2', 'Versão atualizada do hero shooter.', 2022, 99.90, 10 UNION ALL
    SELECT 'Hearthstone', 'Jogo digital de cartas da Blizzard.', 2014, 59.90, 10 UNION ALL
    SELECT 'Heroes of the Storm', 'MOBA com personagens das franquias Blizzard.', 2015, 69.90, 7 UNION ALL
    SELECT 'The Lost Vikings', 'Puzzle-platformer original da Blizzard.', 1992, 49.90, 3
) AS dados
WHERE NOT EXISTS (
    SELECT 1 FROM tb_jogo WHERE tb_jogo.no_jogo = dados.no_jogo
);

INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM tb_jogo AS jogo
CROSS JOIN tb_genero AS genero
WHERE jogo.no_jogo LIKE 'World of Warcraft%'
  AND genero.no_genero IN ('RPG', 'Aventura', 'MMORPG');

INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM tb_jogo AS jogo
CROSS JOIN tb_genero AS genero
WHERE jogo.no_jogo LIKE 'Diablo%'
  AND genero.no_genero IN ('Ação', 'RPG', 'Aventura');

DELETE tb_jogo_genero
FROM tb_jogo_genero
JOIN tb_jogo ON tb_jogo.co_seq_jogo = tb_jogo_genero.co_jogo
JOIN tb_genero ON tb_genero.co_seq_genero = tb_jogo_genero.co_genero
WHERE (tb_jogo.no_jogo LIKE 'Warcraft%' OR tb_jogo.no_jogo LIKE 'StarCraft%')
  AND tb_genero.no_genero <> 'Estratégia';

INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM tb_jogo AS jogo
CROSS JOIN tb_genero AS genero
WHERE (jogo.no_jogo LIKE 'Warcraft%' OR jogo.no_jogo LIKE 'StarCraft%')
  AND genero.no_genero = 'Estratégia';

INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM tb_jogo AS jogo
CROSS JOIN tb_genero AS genero
WHERE jogo.no_jogo IN ('Overwatch', 'Overwatch 2')
  AND genero.no_genero IN ('Ação', 'Tiro');

INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM tb_jogo AS jogo
CROSS JOIN tb_genero AS genero
WHERE jogo.no_jogo = 'Hearthstone'
  AND genero.no_genero = 'Cartas';

INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM tb_jogo AS jogo
CROSS JOIN tb_genero AS genero
WHERE jogo.no_jogo = 'Heroes of the Storm'
  AND genero.no_genero = 'MOBA';

INSERT IGNORE INTO tb_jogo_genero (co_jogo, co_genero)
SELECT jogo.co_seq_jogo, genero.co_seq_genero
FROM tb_jogo AS jogo
CROSS JOIN tb_genero AS genero
WHERE jogo.no_jogo = 'The Lost Vikings'
  AND genero.no_genero IN ('Puzzle', 'Plataforma', 'Aventura');
