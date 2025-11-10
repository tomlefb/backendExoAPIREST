namespace APIRestCOURS.DTOs;

public record RegisterRequest(
    string Email,
    string Password,
    string Nom,
    string Prenom,
    DateTime DateNaissance
);
