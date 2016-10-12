#ifndef ___FREQUENCY_TABLE_HEADER___
#define ___FREQUENCY_TABLE_HEADER___


/**
 * Datastructuren
 */

typedef struct{
	char c;
	unsigned int n;
} ft_entry;

typedef struct{
	ft_entry* ft;
	unsigned int size;
} frequency_table;


/**
 * Interfacefuncties
 */

frequency_table* ft_create(char* encrypted);

void ft_free(frequency_table* ft);

void ft_insert(const char c, frequency_table* ft);

ft_entry* ft_getmax(frequency_table* ft);

char* decrypt(frequency_table* ft, const char* language, const char* encrypted);

/**
 * Hulpfuncties
 */

void ft_fixup(ft_entry* fc, const unsigned int k, const frequency_table* ft);

void ft_fixdown(ft_entry* fc, const unsigned int k, const frequency_table* ft);

void ft_swap(ft_entry* fc1, ft_entry* fc2);

unsigned int get_max_index(double* list, unsigned int size);

#endif
