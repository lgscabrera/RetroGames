-- Etapa 5: testes de integridade
-- Execute com: mysql ... --force < database/04_testes_integridade.sql
-- (--force continua depois de cada erro e mostra o número de cada um)

-- Teste 1: jogo com co_plataforma inexistente
-- Resultado: Erro 1452 (chave estrangeira)
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Teste FK', 1990, 10.00, 1, TRUE, UTC_TIMESTAMP(), 999, 1);

-- Teste 2: plataforma com nome já cadastrado
-- Resultado: Erro 1062 (UNIQUE)
INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
VALUES ('Mega Drive', 'Sega', 1988);

-- Teste 3: excluir plataforma que possui jogos
-- Resultado: Erro 1451 (chave estrangeira)
DELETE FROM tb_plataforma WHERE co_seq_plataforma = 1;

-- Teste 4: jogo com no_jogo nulo
-- Resultado: Erro 1048 (NOT NULL)
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES (NULL, 1990, 10.00, 1, TRUE, UTC_TIMESTAMP(), 1, 1);

-- Teste 5: jogo com vl_preco igual a 0
-- Resultado: aceito (a regra fica no Service, RN06)
INSERT INTO tb_jogo (no_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
VALUES ('Teste preço zero', 1990, 0, 1, TRUE, UTC_TIMESTAMP(), 1, 1);

-- Limpeza dos dados de teste
DELETE FROM tb_jogo WHERE no_jogo = 'Teste preço zero';
