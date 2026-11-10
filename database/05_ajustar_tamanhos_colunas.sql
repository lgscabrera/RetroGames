-- Ajusta os tamanhos das colunas ao modelo relacional do professor.
ALTER TABLE tb_condicao_jogo MODIFY ds_condicao_jogo VARCHAR(50) NOT NULL;
ALTER TABLE tb_perfil MODIFY ds_perfil VARCHAR(50) NOT NULL;
ALTER TABLE tb_usuario
    MODIFY ds_email VARCHAR(256) NOT NULL,
    MODIFY ds_senha_hash VARCHAR(500) NOT NULL;
