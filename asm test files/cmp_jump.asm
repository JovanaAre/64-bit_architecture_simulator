MOV RAX, 10

LOOP:
ADD RAX, 1
CMP RAX, 12 ;poredjenje vrijednosti sadrzane u registru sa konstantom 12
JNE LOOP ;ako gornji uslov nije zadovoljen (vrijednost u RAX != 12) => skok na labelu LOOP
