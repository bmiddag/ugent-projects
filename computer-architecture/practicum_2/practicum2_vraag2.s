lucas:
	pushl	%ebp
	movl	%esp, %ebp
	subl	$16, %esp
	movl	$0, -4(%ebp)
	cmpl	$1, 8(%ebp)
	jg	.L2
	movl	$2, -4(%ebp)
	jmp	.L3
.L2:
	movl	8(%ebp), %eax
	subl	$1, %eax
	movl	%eax, (%esp)
	call	lucas
	movl	%eax, -8(%ebp)
	movl	8(%ebp), %eax
	subl	$2, %eax
	movl	%eax, (%esp)
	call	lucas
	movl	%eax, -12(%ebp)
	movl	-12(%ebp), %eax
	addl	%eax, %eax
	addl	-8(%ebp), %eax
	movl	%eax, -4(%ebp)
.L3:
	movl	-4(%ebp), %eax
	leave
	ret