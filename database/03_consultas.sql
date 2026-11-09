-- Etapa 5: consultas do catálogo (ids e termos de busca são exemplos)

-- 1. Catálogo: jogos ativos com plataforma e condição, ordenados por nome
SELECT j.co_seq_jogo, j.no_jogo, p.no_plataforma, c.ds_condicao_jogo, j.vl_preco, j.qt_estoque
FROM tb_jogo j
JOIN tb_plataforma p ON p.co_seq_plataforma = j.co_plataforma
JOIN tb_condicao_jogo c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo;

-- 2. Gêneros de um jogo informado (jogo 2)
SELECT g.co_seq_genero, g.no_genero
FROM tb_jogo_genero jg
JOIN tb_genero g ON g.co_seq_genero = jg.co_genero
WHERE jg.co_jogo = 2
ORDER BY g.no_genero;

-- 3. Jogos de um gênero informado (gênero 1 = Plataforma)
SELECT j.co_seq_jogo, j.no_jogo
FROM tb_jogo_genero jg
JOIN tb_jogo j ON j.co_seq_jogo = jg.co_jogo
WHERE jg.co_genero = 1
ORDER BY j.no_jogo;

-- 4. Quantidade de jogos por plataforma, incluindo as sem jogos (Neo Geo = 0)
SELECT p.no_plataforma, COUNT(j.co_seq_jogo) AS qt_jogos
FROM tb_plataforma p
LEFT JOIN tb_jogo j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5. Busca por parte do nome, 5 itens por página
-- Total de itens encontrados
SELECT COUNT(*) AS total FROM tb_jogo WHERE no_jogo LIKE CONCAT('%', 'o', '%');
-- Página 1
SELECT co_seq_jogo, no_jogo, vl_preco FROM tb_jogo
WHERE no_jogo LIKE CONCAT('%', 'o', '%')
ORDER BY no_jogo LIMIT 5 OFFSET 0;
-- Página 2
SELECT co_seq_jogo, no_jogo, vl_preco FROM tb_jogo
WHERE no_jogo LIKE CONCAT('%', 'o', '%')
ORDER BY no_jogo LIMIT 5 OFFSET 5;
